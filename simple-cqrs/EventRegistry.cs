using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SimpleCQRS
{
   public interface IEventRegistry
   {
      Option<Type> EventType(string name, int version);
      Option<(string Name, int Version)> EventRevision(Type eventType);
   }

   public sealed class EventRegistry : IEventRegistry
   {
      private readonly Map<string, Type> _revisionTypeMap;
      private readonly Map<string, (string Name, int Version)> _typeRevisionMap;

      public EventRegistry(Seq<(string eventName, int eventVersion, Type eventDataType)> mappings)
      {
         _revisionTypeMap =
            mappings
               .Map<(string, Type)>(x => (RevisionKey(x.eventName, x.eventVersion), x.eventDataType))
               .Apply(x => toMap(x));

         _typeRevisionMap =
            mappings
               .Map<(string, (string, int))>(x => (x.eventDataType.AssemblyQualifiedName, (x.eventName, x.eventVersion)))
               .Apply(x => toMap(x));
      }

      public Option<Type> EventType(string eventName, int eventVersion) =>
         Try(() => _revisionTypeMap[RevisionKey(eventName, eventVersion)])
            .ToOption();

      public Option<(string Name, int Version)> EventRevision(Type eventDataType) =>
         Try(() => _typeRevisionMap[eventDataType.AssemblyQualifiedName])
            .ToOption();

      private static string RevisionKey(string name, int version) =>
         $"{name}-{version}";

      public static readonly Type EventDataMarker = typeof(IEventData);

      public static EventRegistry Cons(Seq<Assembly> assemblies) =>
         assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => EventDataMarker.IsAssignableFrom(x) && x.IsClass && x.IsSealed)
            .ToSeq()
            .Map(ToNameVersionType)
            .Apply(Cons);

      public static EventRegistry Cons(Seq<(string eventName, int eventVersion, Type eventDataType)> mappings) =>
         new EventRegistry(mappings);

      private static (string Name, int Version, Type EventDataType) ToNameVersionType(Type eventDataType) =>
         eventDataType
            .GetCustomAttributes(typeof(EventDataAttribute), false)
            .ToSeq()
            .Map(x => x as EventDataAttribute)
            .Head()
            .Apply(x => (x.Name, x.Version, eventDataType));
   }
}
