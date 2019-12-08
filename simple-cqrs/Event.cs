using System;
using LanguageExt;

namespace SimpleCQRS
{
   public sealed class Event
   {
      public AggregateInfo AggregateInfo { get; }
      public DateTimeOffset Timestamp { get; }
      public string Owner { get; }
      public IEventData EventData { get; }

      public Event(AggregateInfo info, DateTimeOffset timestamp, string owner, IEventData eventData)
      {
         AggregateInfo = info;
         Timestamp = timestamp;
         Owner = owner ?? "";
         EventData = eventData;
      }
   }
}
