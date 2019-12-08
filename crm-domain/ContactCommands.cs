using System;
using SimpleCQRS;
using LanguageExt;
using CRM.Domain.Types;

namespace CRM.Domain
{
   public sealed class ReadContact : ReadAggregate<Contact>
   {
      public ReadContact(Guid rootId) : base(rootId)
      {
      }
   }

   public sealed class CreateContact : CreateRootCommand
   {
      public PersonalName Name { get; }

      public CreateContact(PersonalName name) =>
         Name = name;
   }

   public sealed class RenameContact : Command
   {
      public PersonalName Name { get; }

      public RenameContact(Guid id, int originalVersion, PersonalName name) 
         : base(id, originalVersion) =>
            Name = name;
   }

   public sealed class DeleteContact : Command
   {
      public DeleteContact(Guid id, int originalVersion) 
         : base(id, originalVersion)
      { 
      }
   }

   
   public sealed class AddContactPhone : DetailCommand
   {
      public PhoneNumber Phone { get; }

      public AddContactPhone(Guid contactId, int originalVersion, PhoneNumber phone)
         : base(contactId, originalVersion) =>
            Phone = phone;
   }

   public sealed class UpdateContactPhone : DetailCommand
   {
      public PhoneNumber Phone { get; }

      public UpdateContactPhone(Guid contactId, int originalVersion, Guid phoneId, PhoneNumber phone) 
         : base(contactId, phoneId, originalVersion) =>
            Phone = phone;
   }

   public sealed class DeleteContactPhone : DetailCommand
   {
      public DeleteContactPhone(Guid contactId, int originalVersion, Guid phoneId) 
         : base(contactId, phoneId, originalVersion)
      {
      }
   }


   public sealed class AddContactAddress : DetailCommand
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

   public sealed class UpdateContactAddress : DetailCommand
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

   public sealed class DeleteContactAddress : DetailCommand
   {
      public DeleteContactAddress(Guid contactId, int originalVersion, Guid addressId)
         : base(contactId, addressId, originalVersion)
      {
      }
   }
}
