using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleCQRS;
using LanguageExt;
using System.Net.Mime;

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

      [HttpPost]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status201Created)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> Create(CreateContact model)
      {
         var cmd = new Domain.Types.PersonalName(model.Given, model.Middle, model.Family)
            .Apply(x => new Domain.CreateContact(x));

         var sent = _bus.Send(cmd);

         var result = await cmd.Result.Wait();

         return result.Value.Match(
           Right: _ => CreatedAtAction(nameof(GetByRootId), new { rootId = cmd.RootId }, null),

            // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
      }

      [HttpPut("{rootId}")]
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult> Rename(Guid rootId, [FromBody] RenameContact model)
      {
         var cmd = new Domain.Types.PersonalName(model.Given, model.Middle, model.Family)
            .Apply(x => new Domain.RenameContact(rootId, model.OriginalVersion, x));

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
         var cmd = new Domain.ReadContact(rootId);
         var sent = _bus.Send(cmd);
         var result = await cmd.Result.Wait();

         return result.Value.Match(

           // TODO: make a nice DTO to expose
           Right: contact => new JsonResult(new { aggregate = contact.Item1, events = contact.Item2 }),

           // TODO: don't bubble up any native exception messages
           Left: error => BadRequest(error) as ActionResult);
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
}