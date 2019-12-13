using System;
using CRM.Domain.Types;
using SimpleCQRS;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CRM.Domain.Aggregates
{
   public sealed class Company : Aggregate
   {
      public CompanyName Name { get; private set; }

      public Company(Seq<Event> history)
         : base(history, (a, i, e) => Apply(a, i, e))
      {
      }

      public Company(Seq<Event> history, Event newEvent)
         : base(history, newEvent, (a, i, e) => Apply(a, i, e))
      {
      }

      private static void Apply(Aggregate aggregate, int version, Event e)
      {
         var company = aggregate as Company;
         if (company == null) throw new Exception("Company.Apply provided invalid aggregate");

         switch (e.EventData)
         {
            case CompanyCreated created:
               if (version != 1) throw new Exception("event CompanyCreated is only valid for version 1");
               company.Name = created.Name;
               break;
         }
      }
   }
}
