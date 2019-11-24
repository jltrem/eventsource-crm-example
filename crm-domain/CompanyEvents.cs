using System;
using CRM.Domain.Types;
using SimpleCQRS;

namespace CRM.Domain
{
   public class CompanyCreated : IEventData
   {
      public CompanyName Name { get; }

      public CompanyCreated(CompanyName name)
      {
         Name = name;
      }
   }
}
