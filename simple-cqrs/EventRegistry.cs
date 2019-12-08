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

      public EventRegistry(Seq<(string name, int version, Type type)> mappings)
      {
         _revisionTypeMap =
            mappings
               .Map<(string, Type)>(x => (RevisionKey(x.name, x.version), x.type))
               .Apply(x => toMap(x));

         _typeRevisionMap =
            mappings
               .Map<(string, (string, int))>(x => (x.type.AssemblyQualifiedName, (x.name, x.version)))
               .Apply(x => toMap(x));
      }

      public Option<Type> EventType(string name, int version) =>
         Try(() => _revisionTypeMap[RevisionKey(name, version)])
            .ToOption();

      public Option<(string Name, int Version)> EventRevision(Type eventType) =>
         Try(() => _typeRevisionMap[eventType.AssemblyQualifiedName])
            .ToOption();

      private static string RevisionKey(string name, int version) =>
         $"{name}-{version}";

      public static readonly Type EventDataMarker = typeof(IEventData);

      public static EventRegistry Cons(Seq<Assembly> assemblies) =>
         assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => EventDataMarker.IsAssignableFrom(p) && p.IsClass && p.IsSealed)
            .ToSeq()
            .Map(eType => eType
               .GetCustomAttributes(typeof(EventDataAttribute), false)
               .ToSeq()
               .Map(x => x as EventDataAttribute)
               .Head()
               .Apply(x => (x.Name, x.Version, eType)))
            .Apply(x => new EventRegistry(x));
   }
}
