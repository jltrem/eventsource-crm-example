using System;
using LanguageExt;

namespace SimpleCQRS
{
   /// <summary>
   /// This marks an event data class.
   /// </summary>
   public interface IEventData { }

   /// <summary>
   /// All IEventData classes must be marked with this attribute
   /// which specifies revision.
   /// </summary>
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
}
