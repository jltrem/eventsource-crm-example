using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleCQRS
{
   public class EventDTO : Record<EventDTO>
   {
      public string Name { get; }
      public int Version { get; }
      public object Data { get; }
      public Type DataType { get; }

      public EventDTO(string name, int version, object data, Type dataType)
      {
         Name = name;
         Version = version;
         Data = data;
         DataType = dataType;
      }
   }

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


   public class EventForStorage : Record<EventForStorage>
   {
      public AggregateInfo Aggregate { get; }
      public DateTimeOffset Timestamp { get; }
      public string Owner { get; }
      public EventDTO DTO { get; }

      public EventForStorage(AggregateInfo aggregate, DateTimeOffset timestamp, string owner, EventDTO dto)
      {
         Aggregate = aggregate;
         Timestamp = timestamp;
         Owner = owner ?? "";
         DTO = dto;
      }
   }

   /// <summary>
   /// This marks an event data class, but not its DTO.
   /// </summary>
   public interface IEventData 
   { 
   }

   public class Event
   {
      public AggregateInfo AggregateInfo { get; }
      public DateTimeOffset Timestamp { get; }
      public string Owner { get; }
      public IEventData EventData { get; }


      public Event(AggregateInfo info, DateTimeOffset timestamp, string owner, IEventData data)
      {
         AggregateInfo = info;
         Timestamp = timestamp;
         Owner = owner ?? "";
         EventData = data;
      }
   }
}
