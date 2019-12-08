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
      public const string PhoneAdded = "contact-phone-added";
      public const string PhoneUpdated = "contact-phone-updated";
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

   [EventData(ContactEvent.PhoneAdded, 1)]
   public sealed class ContactPhoneAdded : IEventData
   {
      public Guid PhoneId { get; }
      public PhoneNumber Phone { get; }

      public ContactPhoneAdded(Guid phoneId, PhoneNumber phone)
      {
         PhoneId = phoneId;
         Phone = phone;
      }
   }

   [EventData(ContactEvent.PhoneUpdated, 1)]
   public sealed class ContactPhoneUpdated : IEventData
   {
      public Guid PhoneId { get; }
      public PhoneNumber Phone { get; }

      public ContactPhoneUpdated(Guid phoneId, PhoneNumber phone)
      {
         PhoneId = phoneId;
         Phone = phone;
      }
   }
}
