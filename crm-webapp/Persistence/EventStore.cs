using SimpleCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CRM.Persistence
{
   public class EventStore : IEventStore
   {
      private readonly EventStoreContext _db;
      private readonly IEventRegistry _registry;

      public EventStore(EventStoreContext db, IEventRegistry registry)
      {
         _db = db;
         _registry = registry;
      }

      public Either<string, Seq<Event>> GetEvents(Guid rootId) =>
         Try(() =>
            _db.AggregateEvents
               .Where(x => x.RootId == rootId)
               .OrderBy(x => x.AggregateVersion)
               .ToList()
               .Map(x =>              
                  ToDtoType(x.EventName, x.EventVersion)
                     .Apply(type => ToEvent(x, type))
               )
               .ToSeq()
         )
         .Match<Seq<Event>, Either<string, Seq<Event>>>(
            Succ: x => Right(x),
            Fail: ex => Left(ex.Message));


      private Type ToDtoType(string eventName, int eventVersion) =>
         _registry.EventType(eventName, eventVersion)
            .IfNone(() => throw new Exception($"event revision not registered: ({eventName}, {eventVersion})"));

      private Event ToEvent(AggregateEvent aggEvent, Type dtoType)
      {        
         var dto = JsonConvert.DeserializeObject(aggEvent.EventData, dtoType) as IEventData;
         if (dto == null) throw new Exception($"event could not be deserialized: (rootId={aggEvent.RootId}, version={aggEvent.AggregateVersion})");

         var info = new AggregateInfo(aggEvent.AggregateName, aggEvent.RootId, aggEvent.AggregateVersion);
         return new Event(info, aggEvent.Timestamp, aggEvent.Owner, dto);
      }

      public Either<string, Unit> AddEvent(Event e) =>
         Try(() => e.EventData.GetType()
            .Apply(dtoType => _registry.EventRevision(dtoType).Match(
               Some: revision =>
               {
                  var data = new AggregateEvent
                  {
                     RootId = e.AggregateInfo.RootId,
                     AggregateVersion = e.AggregateInfo.Version,
                     AggregateName = e.AggregateInfo.Name,
                     EventName = revision.Name,
                     EventVersion = revision.Version,
                     EventData = JsonConvert.SerializeObject(e.EventData, dtoType, new JsonSerializerSettings
                     {
                        Formatting = Formatting.None,
                        TypeNameHandling = TypeNameHandling.None
                     }),
                     Timestamp = e.Timestamp,
                     Owner = e.Owner
                  };

                  _db.AggregateEvents.Add(data);
                  return Unit.Default;
               },
               None: () => throw new Exception($"event not registered for aggregate: (rootId={e.AggregateInfo.RootId}, version={e.AggregateInfo.Version})"))
            )
         )
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
