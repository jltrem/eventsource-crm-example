using System;
using CRM.Domain.Types;
using SimpleCQRS;

namespace CRM.Domain
{
   internal static class CompanyEvent
   {
      public const string AggregateName = "company";

      public const string Created = "company-created";
      public const string Renamed = "company-renamed";
   }

   [EventData(CompanyEvent.Created, 1)]
   public sealed class CompanyCreated : IEventData
   {
      public CompanyName Name { get; }

      public CompanyCreated(CompanyName name)
      {
         Name = name;
      }
   }
}
