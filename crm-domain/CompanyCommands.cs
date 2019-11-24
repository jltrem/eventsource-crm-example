using System;
using CRM.Domain.Types;
using SimpleCQRS;

namespace CRM.Domain
{
   public class CreateCompany : CreateRootCommand
   {
      public CompanyName Name { get; }

      public CreateCompany(CompanyName name) =>
         Name = name;
   }

   public class RenameCompany : Command
   {
      public CompanyName Name { get; }

      public RenameCompany(Guid id, int originalVersion, CompanyName name) 
         : base(id, originalVersion) =>
            Name = name;
   }

   public class DeleteCompany : Command
   {
      public DeleteCompany(Guid id, int originalVersion) 
         : base(id, originalVersion)
      { 
      }
   }
}
