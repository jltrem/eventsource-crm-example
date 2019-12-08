using System;
using LanguageExt;

namespace SimpleCQRS
{
   public interface IEventStore
   {
      Either<string, Seq<Event>> GetEvents(Guid rootId);
      Either<string, Unit> AddEvent(Event e);
      Either<string, Unit> Save();
   }
}
