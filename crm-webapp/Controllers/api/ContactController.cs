using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleCQRS;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Net.Mime;
using CRM.Domain.DTO;

namespace CRM.Webapp.Controllers.api
{
   [Route("api/contact")]
   [ApiController]
   public class ContactController : ControllerBase
   {
      private readonly ICommandBus _bus;

      public ContactController(ICommandBus bus)
      {
         _bus = bus;
      }

      [HttpPost("create")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status201Created)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> Create(CreateContact model)
      {
         var cmd = new Domain.Types.PersonalName(model.Given, model.Middle, model.Family)
            .Apply(x => new Domain.Aggregates.CreateContact(x));

         var sent = _bus.Send(cmd);

         var result = await cmd.Result.Wait();

         return result.Value.Match(
           Right: _ => CreatedAtAction(nameof(GetByRootId), new { rootId = cmd.RootId }, null),

            // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
      }

      [HttpPut("{rootId}/rename")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> Rename(Guid rootId, [FromBody] RenameContact model)
      {
         var cmd = new Domain.Types.PersonalName(model.Given, model.Middle, model.Family)
            .Apply(x => new Domain.Aggregates.RenameContact(rootId, model.OriginalVersion, x));

         var sent = _bus.Send(cmd);

         var result = await cmd.Result.Wait();

         return result.Value.Match(
           Right: _ => Ok(),

           // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
      }

      [HttpPut("{rootId}/add-phone")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> AddPhone(Guid rootId, [FromBody] AddOrUpdatePhone model)
      {
         var cmd = new Domain.Types.PhoneNumber(model.PhoneType, model.Number, model.Ext)
            .Apply(x => new Domain.Aggregates.AddContactPhone(rootId, model.OriginalVersion, x));

         var sent = _bus.Send(cmd);

         var result = await cmd.Result.Wait();

         return result.Value.Match(
           Right: _ => Ok(),

           // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
      }

      [HttpPut("{rootId}/update-phone/{phoneId}")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> AddPhone(Guid rootId, Guid phoneId, [FromBody] AddOrUpdatePhone model)
      {
         var cmd = new Domain.Types.PhoneNumber(model.PhoneType, model.Number, model.Ext)
            .Apply(x => new Domain.Aggregates.UpdateContactPhone(rootId, model.OriginalVersion, phoneId, x));

         var sent = _bus.Send(cmd);

         var result = await cmd.Result.Wait();

         return result.Value.Match(
           Right: _ => Ok(),

           // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
      }

      [HttpGet("{rootId}")]
      public async Task<ActionResult> GetByRootId(Guid rootId)
      {
         var cmd = new Domain.Aggregates.ReadContact(rootId);
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
   }

}