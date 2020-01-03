using System;
using LanguageExt;
using static LanguageExt.FSharp;
using Fescq;
using static Fescq.EventRegistryFuncs;

namespace CRM.Webapp
{
   public class CrmEventRegistry
   {
      private readonly EventRegistry _registry;

      public CrmEventRegistry()
      {
         var assemblies = AppDomain.CurrentDomain.GetAssemblies();
         _registry = createForAssemblies(assemblies);         
      }

      public Option<Type> EventType(string name, int version) =>
         fs(eventType(_registry, name, version));

      public Option<(string Name, int Version)> EventRevision(Type dataType) =>
         fs(eventRevision(_registry, dataType));
   }
}
