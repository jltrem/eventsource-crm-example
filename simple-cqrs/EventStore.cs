using System;
using System.Collections.Generic;
using LanguageExt;

namespace SimpleCQRS
{
   public interface IEventStore
   {
      Either<string, Seq<Event>> GetEvents(Guid rootId);

      Either<string, Unit> AddEvent(EventForStorage e);
      Either<string, Unit> Save();
   }
}
