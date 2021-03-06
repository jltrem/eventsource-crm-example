﻿using System;
using System.Collections.Generic;
using CRM.Domain.Types;
using SimpleCQRS;
using LanguageExt;
using static LanguageExt.Prelude;


namespace CRM.Domain.Aggregates
{
   public sealed class Contact : Aggregate
   {
      public PersonalName Name { get; private set; }

      public Map<Guid, PhoneNumber> Phones { get; private set; }

      public Contact(Seq<Event> history) 
         : base(history, (a, i, e) => Apply(a, i, e))
      {
      }

      public Contact(Seq<Event> history, Event newEvent) 
         : base(history, newEvent, (a, i, e) => Apply(a, i, e))
      {
      }

      private static void Apply(Aggregate aggregate, int version, Event e)
      {
         var contact = aggregate as Contact;
         if (contact == null) throw new Exception("Contact.Apply provided invalid aggregate");

         switch (e.EventData)
         {
            case ContactCreated created:
               if (version != 1) throw new Exception("event ContactCreated is only valid for version 1");
               contact.Name = created.Name;
               break;

            case ContactRenamed renamed:
               contact.Name = renamed.Name;
               break;

            case ContactPhoneAdded phoneAdded:
               contact.Phones = contact.Phones.Add(phoneAdded.PhoneId, phoneAdded.Phone);
               break;

            case ContactPhoneUpdated phoneUpdated:
               if (!contact.Phones.ContainsKey(phoneUpdated.PhoneId)) throw new Exception("event ContactPhoneUpdated id not found");
               contact.Phones = contact.Phones.AddOrUpdate(phoneUpdated.PhoneId, phoneUpdated.Phone);
               break;

            default:
               throw new Exception("Contact.Apply provided unexpected event");
         }
      }
   }
}
