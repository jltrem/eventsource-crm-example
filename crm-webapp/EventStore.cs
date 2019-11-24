using SimpleCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CRM.Webapp
{
   public class EventStore : IEventStore
   {
      private readonly EventStoreContext _db;

      public EventStore(EventStoreContext db)
      {
         _db = db;
      }

      public Either<string, Seq<Event>> GetEvents(Guid rootId) =>
         Try(() =>
            _db.AggregateEvents
               .Where(x => x.RootId == rootId)
               .OrderBy(x => x.AggregateVersion)
               .Map(x => new Event(
                  new AggregateInfo(x.AggregateName, rootId, x.AggregateVersion),
                  x.Timestamp,
                  x.Owner,
                  JsonConvert.DeserializeObject(x.Data, Type.GetType(x.DataType)) as IEventData))
               .ToSeq()
         )
         .Match<Seq<Event>, Either<string, Seq<Event>>>(
            Succ: x => Right(x),
            Fail: ex => Left(ex.Message));

      public Either<string, Unit> AddEvent(EventForStorage e) =>
         Try(() =>
         {
            var data = new AggregateEvent
            {
               RootId = e.Aggregate.RootId,
               AggregateVersion = e.Aggregate.Version,
               AggregateName = e.Aggregate.Name,
               EventName = e.DTO.Name,
               EventVersion = e.DTO.Version,
               DataType = e.DTO.DataType.AssemblyQualifiedName,
               Data = JsonConvert.SerializeObject(e.DTO.Data, e.DTO.DataType, new JsonSerializerSettings
               {
                  Formatting = Formatting.None,
                  TypeNameHandling = TypeNameHandling.None
               }),
               Timestamp = e.Timestamp,
               Owner = e.Owner
            };

            _db.AggregateEvents.Add(data);
            return Unit.Default;
         })
         .Match<Unit, Either<string, Unit>>(
            Succ: x => Right(x),
            Fail: ex => Left(ex.Message));

      public Either<string, Unit> Save() =>
         Try(() =>
         {            
            _db.SaveChanges();
            return Unit.Default;
         })
         .Match<Unit, Either<string, Unit>>(
            Succ: x => Right(x),
            Fail: ex => Left(ex.Message));

   }
}
