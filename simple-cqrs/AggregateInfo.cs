using System;
using LanguageExt;

namespace SimpleCQRS
{
   public sealed class AggregateInfo : Record<AggregateInfo>
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
}
