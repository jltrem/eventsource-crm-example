using System;
using SimpleCQRS;
using LanguageExt;
using CRM.Domain.Types;

namespace CRM.Domain
{
   public class ReadContact : ReadAggregate<Contact>
   {
      public ReadContact(Guid rootId) : base(rootId)
      {
      }
   }

   public class CreateContact : CreateRootCommand
   {
      public PersonalName Name { get; }

      public CreateContact(PersonalName name) =>
         Name = name;
   }

   public class RenameContact : Command
   {
      public PersonalName Name { get; }

      public RenameContact(Guid id, int originalVersion, PersonalName name) 
         : base(id, originalVersion) =>
            Name = name;
   }

   public class DeleteContact : Command
   {
      public DeleteContact(Guid id, int originalVersion) 
         : base(id, originalVersion)
      { 
      }
   }

   
   public class AddContactPhone : DetailCommand
   {
      public PhoneNumber PhoneNumber { get; }

      public AddContactPhone(Guid contactId, int originalVersion, PhoneType type, string number, string ext) 
         : base(contactId, originalVersion) =>
            PhoneNumber = new PhoneNumber(type, number, ext);     
   }

   public class UpdateContactPhone : DetailCommand
   {
      public PhoneNumber PhoneNumber { get; }

      public UpdateContactPhone(Guid contactId, int originalVersion, Guid phoneId, PhoneNumber phoneNumber) 
         : base(contactId, phoneId, originalVersion) =>
            PhoneNumber = phoneNumber;
   }

   public class DeleteContactPhone : DetailCommand
   {
      public DeleteContactPhone(Guid contactId, int originalVersion, Guid phoneId) 
         : base(contactId, phoneId, originalVersion)
      {
      }
   }


   public class AddContactAddress : DetailCommand
   {
      public PhysicalAddressUsage Usage { get; }
      public PhysicalAddress Address { get; }

      public AddContactAddress(Guid contactId, int originalVersion, PhysicalAddressUsage usage, PhysicalAddress address)
         : base(contactId, originalVersion)
      {
         Usage = usage;
         Address = address;
      }
   }

   public class UpdateContactAddress : DetailCommand
   {
      public PhysicalAddressUsage Usage { get; }
      public PhysicalAddress Address { get; }

      public UpdateContactAddress(Guid contactId, int originalVersion, Guid addressId, PhysicalAddressUsage usage, PhysicalAddress address)
         : base(contactId, addressId, originalVersion)
      {
         Usage = usage;
         Address = address;
      }
   }

   public class DeleteContactAddress : DetailCommand
   {
      public DeleteContactAddress(Guid contactId, int originalVersion, Guid addressId)
         : base(contactId, addressId, originalVersion)
      {
      }
   }
}
