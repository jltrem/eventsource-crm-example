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
using Domain = CRM.Domain;
using ES = Fescq.EventStoreCSharp;

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
                  return CreatedAtAction(nameof(GetByRootId), new { rootId = aggregate.Key.Id }, null);
               },
               None: () => BadRequest(fs(error).IfNone("unknown error")) as ActionResult);
         }
         else
         {
            return BadRequest("could not create contact");
         }
      }

      [HttpPut("{rootId}/rename")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> Rename(Guid rootId, [FromBody] RenameContact model)
      {
         throw new NotImplementedException();
         /*
         var cmd = new Domain.PersonalName(model.Given, model.Middle, model.Family)
            .Apply(x => new Domain.Aggregate.Contact.RenameContact(rootId, model.OriginalVersion, x));

         var sent = _bus.Send(cmd);

         var result = await cmd.Result.Wait();

         return result.Value.Match(
           Right: _ => Ok(),

           // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
           */
      }

      [HttpPut("{rootId}/add-phone")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> AddPhone(Guid rootId, [FromBody] AddOrUpdatePhone model)
      {
         throw new NotImplementedException();
         /*

         var cmd = new Domain.PhoneNumber(model.PhoneTypeAsEnum(), model.Number, model.Ext)
            .Apply(x => new Domain.Aggregate.Contact.AddContactPhone(rootId, model.OriginalVersion, x));

         var sent = _bus.Send(cmd);

         var result = await cmd.Result.Wait();

         return result.Value.Match(
           Right: _ => Ok(),

           // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
           */
      }

      [HttpPut("{rootId}/update-phone/{phoneId}")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> AddPhone(Guid rootId, Guid phoneId, [FromBody] AddOrUpdatePhone model)
      {
         throw new NotImplementedException();
         /*

         var cmd = new Domain.PhoneNumber(model.PhoneTypeAsEnum(), model.Number, model.Ext)
            .Apply(x => new Domain.Aggregate.Contact.UpdateContactPhone(rootId, model.OriginalVersion, phoneId, x));

         var sent = _bus.Send(cmd);

         var result = await cmd.Result.Wait();

         return result.Value.Match(
           Right: _ => Ok(),

           // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
           */
      }


      [HttpGet("{rootId}")]
      public async Task<ActionResult> GetByRootId(Guid rootId)
      {
         throw new NotImplementedException();
         /*
         var cmd = new Domain.Aggregate.Contact.ReadContact(rootId);
         var sent = _bus.Send(cmd);
         var result = await cmd.Result.Wait();

         return result.Value.Match(

            Right: contact =>
            {
               var agg = head(contact);
               var data = Contact.Cons(agg);
               return new Aggregate<Contact>(agg.Info, data)
                  .Apply(ToJsonContent);
            },

           // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
*/
      }


      private readonly Newtonsoft.Json.JsonSerializerSettings _jsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
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