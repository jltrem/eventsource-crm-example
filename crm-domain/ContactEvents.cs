using System;
using CRM.Domain.Types;
using SimpleCQRS;

namespace CRM.Domain
{
   public sealed class ContactCreated : IEventData
   {
      public PersonalName Name { get; }

      public ContactCreated(PersonalName name)
      {
         Name = name;
      }
   }

   public sealed class ContactRenamed : IEventData
   {
      public PersonalName Name { get; }

      public ContactRenamed(PersonalName name)
      {
         Name = name;
      }
   }

}
