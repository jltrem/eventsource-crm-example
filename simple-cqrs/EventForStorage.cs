using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleCQRS
{
   public class AggregateInfo : Record<AggregateInfo>
   {
      public string Name { get; }
      public Guid RootId { get; }
      public int Version { get; }

      public AggregateInfo(string name, Guid rootId, int version)
      {
         Name = name;
         Version = version;
         RootId = rootId;
      }
   }

   /// <summary>
   /// This marks an event data class.
   /// </summary>
   public interface IEventData { }

   public static class EventDataExt
   {
      public static (string Name, int Version) Revision(this IEventData e) =>
         e.GetType()
            .GetCustomAttributes(typeof(EventDataAttribute), false)
            .ToSeq()
            .Map(x => x as EventDataAttribute)
            .Head()
            .Apply(x => (x.Name, x.Version));
   }

   [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
   public sealed class EventDataAttribute : Attribute
   {
      public string Name { get; }
      public int Version { get; }

      public EventDataAttribute(string name, int version)
      {
         Name = name;
         Version = version;
      }
   }

   public class Event
   {
      public AggregateInfo AggregateInfo { get; }
      public DateTimeOffset Timestamp { get; }
      public string Owner { get; }
      public IEventData Data { get; }


      public Event(AggregateInfo info, DateTimeOffset timestamp, string owner, IEventData data)
      {
         AggregateInfo = info;
         Timestamp = timestamp;
         Owner = owner ?? "";
         Data = data;
      }
   }
}
