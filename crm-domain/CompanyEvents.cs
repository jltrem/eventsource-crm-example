using System;
using CRM.Domain.Types;
using SimpleCQRS;

namespace CRM.Domain
{
   internal static class CompanyEventName
   {
      public const string Created = "company-created";
      public const string Renamed = "company-renamed";
   }

   [EventData(CompanyEventName.Created, 1)]
   public class CompanyCreated : IEventData
   {
      public CompanyName Name { get; }

      public CompanyCreated(CompanyName name)
      {
         Name = name;
      }
   }
}
