using System;
using CRM.Domain.Types;
using SimpleCQRS;

namespace CRM.Domain
{
   internal static class ContactEventName
   {
      public const string Created = "contact-created";
      public const string Renamed = "contact-renamed";
   }

   [EventData(ContactEventName.Created, 1)]
   public sealed class ContactCreated : IEventData
   {
      public PersonalName Name { get; }

      public ContactCreated(PersonalName name)
      {
         Name = name;
      }
   }

   [EventData(ContactEventName.Renamed, 1)]
   public sealed class ContactRenamed : IEventData
   {
      public PersonalName Name { get; }

      public ContactRenamed(PersonalName name)
      {
         Name = name;
      }
   }

}
