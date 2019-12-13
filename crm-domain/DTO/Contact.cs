using System;
using System.Collections.Generic;
using CRM.Domain.Types;
using SimpleCQRS;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CRM.Domain.DTO
{
   public class Contact
   {
      public PersonalName Name { get; set; }
      public Dictionary<Guid, PhoneNumber> Phones { get; set; }

      public static Contact Cons(Aggregates.Contact contact) =>
         new Contact
         {
            Name = contact.Name,
            Phones = new Dictionary<Guid, PhoneNumber>(contact.Phones.ToDictionary())
         };
   }
}
