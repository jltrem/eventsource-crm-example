using System;
using CRM.Domain.Types;
using SimpleCQRS;

namespace CRM.Domain.Aggregates
{
   public sealed class CreateCompany : CreateRootCommand
   {
      public CompanyName Name { get; }

      public CreateCompany(CompanyName name) =>
         Name = name;
   }

   public sealed class RenameCompany : Command
   {
      public CompanyName Name { get; }

      public RenameCompany(Guid id, int originalVersion, CompanyName name) 
         : base(id, originalVersion) =>
            Name = name;
   }

   public sealed class DeleteCompany : Command
   {
      public DeleteCompany(Guid id, int originalVersion) 
         : base(id, originalVersion)
      { 
      }
   }
}
