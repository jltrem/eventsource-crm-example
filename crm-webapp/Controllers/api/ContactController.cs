using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.FSharp;
using System.Net.Mime;
using Fescq;
using ES = Fescq.EventStoreCSharp;

// TODO: enforce originalVersion concurrency check
// TODO: return aggregateId/version/[detailId] on POST or PUT
// TODO: return aggregateId/version with GET
// TODO: move guts of Update(Guid aggregateId, Command.ICommand cmd) to crm-domain-fs + Fescq
// TODO: metadata

namespace CRM.Webapp.Controllers.api
{
   [Route("api/contact")]
   [ApiController]
   public class ContactController : ControllerBase
   {
      private readonly IEventStore _store;

      public ContactController(CrmEventStore store)
      {
         _store = store.EventStore;
      }

      private DateTimeOffset TimestampNow { get { return DateTimeOffset.UtcNow; } }

      [HttpPost("create")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status201Created)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> Create(CreateContact model)
      {
         var name = new Domain.PersonalName(model.Given, model.Middle, model.Family);
         var cmd = new Domain.Aggregate.Contact.CreateContact(name);

         var (aggregate, events) = Domain.Aggregate.Contact.Handle.create(TimestampNow, "foo meta data", cmd);

         if (events.Length == 1)
         {
            var (ok, error) = ES.AddEvent(_store, events[0]);
            return fs(ok).Match(
               Some: _ =>
               {
                  ES.Save(_store);
                  return CreatedAtAction(nameof(GetAggregate), new { aggregateId = aggregate.Key.Id }, null);
               },
               None: () => BadRequest(fs(error).IfNone("unknown error")) as ActionResult);
         }
         else
         {
            return BadRequest("could not create contact");
         }
      }

      [HttpPut("{aggregateId}/rename")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> Rename(Guid aggregateId, [FromBody] RenameContact model)
      {
         var name = new Domain.PersonalName(model.Given, model.Middle, model.Family);
         var cmd = new Domain.Aggregate.Contact.RenameContact(aggregateId, model.OriginalVersion, name);
         return Update(aggregateId, cmd);
      }
     
      [HttpPut("{aggregateId}/add-phone")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> AddPhone(Guid aggregateId, [FromBody] AddOrUpdatePhone model)
      {
         var phone = new Domain.PhoneNumber(model.PhoneTypeAsEnum(), model.Number, model.Ext);
         var cmd = new Domain.Aggregate.Contact.AddContactPhone(aggregateId, model.OriginalVersion, phone);
         return Update(aggregateId, cmd);
      }

      [HttpPut("{aggregateId}/update-phone/{phoneId}")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> AddPhone(Guid aggregateId, Guid phoneId, [FromBody] AddOrUpdatePhone model)
      {
         var phone = new Domain.PhoneNumber(model.PhoneTypeAsEnum(), model.Number, model.Ext);
         var cmd = new Domain.Aggregate.Contact.UpdateContactPhone(aggregateId, model.OriginalVersion, phoneId, phone);
         return Update(aggregateId, cmd);
      }

      [HttpGet("{aggregateId}")]
      public async Task<ActionResult> GetAggregate(Guid aggregateId)
      {
         var (aggregate, loadError) = CRM.Domain.Aggregate.Contact.Storage.CSharp.Load(_store, aggregateId);
         return fs(aggregate).Match(
            Some: agg => ToJsonContent(agg.Entity),
            None: () => BadRequest(fs(loadError).IfNone("unknown error")) as ActionResult);
      }


      private readonly Newtonsoft.Json.JsonSerializerSettings _jsonSerializerSettings = 
         new Newtonsoft.Json.JsonSerializerSettings
            {
               ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
               Converters = new List<Newtonsoft.Json.JsonConverter>
                  {
                     new Newtonsoft.Json.Converters.StringEnumConverter()
                  }
            };

      private ContentResult ToJsonContent<T>(T value) where T : class =>         
         Newtonsoft.Json.JsonConvert.SerializeObject(value, _jsonSerializerSettings)
            .Apply(json => Content(json, "application/json"));

      private ActionResult Update(Guid aggregateId, Command.ICommand cmd)
      {
         // TODO: move this ugliness to crm-domain-fs & only deal with the HTTP response here
         var (aggregate, loadError) = CRM.Domain.Aggregate.Contact.Storage.CSharp.Load(_store, aggregateId);
         return fs(aggregate).Match(
            Some: agg =>
            {


               var (update, updateError) = Domain.Aggregate.Contact.Handle.CSharp.Update(TimestampNow, "foo meta data", cmd, agg);
               return fs(update).Match(
                  Some: update =>
                  {
                     CRM.Domain.Aggregate.Contact.Storage.save(_store, update.Item1, update.Item2);
                     return Ok();
                  },
                  None: () => BadRequest(fs(updateError).IfNone("unknown error")) as ActionResult);
            },
            None: () => BadRequest(fs(loadError).IfNone("unknown error")) as ActionResult);
      }
   }

   public class CreateContact
   {
      public string Given { get; set;  }
      public string Middle { get; set; }
      public string Family { get; set; }
   }

   public class RenameContact : CreateContact
   {
      public int OriginalVersion { get; set; }
   }

   public class AddOrUpdatePhone
   {
      public int OriginalVersion { get; set; }
      public string PhoneType { get; set; }
      public string Number { get; set; }
      public string Ext { get; set; }

      public Domain.PhoneType PhoneTypeAsEnum() =>
         PhoneType
            .ToLower()
            .Apply(x => x switch
               {
                  "mobile" => Domain.PhoneType.Mobile,
                  "work" => Domain.PhoneType.Work,
                  "home" => Domain.PhoneType.Home,
                  _ => Domain.PhoneType.Unknown
               });
   }

}