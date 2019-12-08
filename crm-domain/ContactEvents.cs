using System;
using CRM.Domain.Types;
using SimpleCQRS;

namespace CRM.Domain
{
   internal static class ContactEvent
   {
      public const string AggregateName = "contact";

      public const string Created = "contact-created";
      public const string Renamed = "contact-renamed";
   }

   [EventData(ContactEvent.Created, 1)]
   public sealed class ContactCreated : IEventData
   {
      public PersonalName Name { get; }

      public ContactCreated(PersonalName name)
      {
         Name = name;
      }
   }

   [EventData(ContactEvent.Renamed, 1)]
   public sealed class ContactRenamed : IEventData
   {
      public PersonalName Name { get; }

      public ContactRenamed(PersonalName name)
      {
         Name = name;
      }
   }

}
