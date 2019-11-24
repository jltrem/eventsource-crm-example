using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static CRM.Domain.Types.StringExt;

namespace CRM.Domain.Types
{
   public enum PhysicalAddressUsage
   {
      Unknown = 0,
      PrimaryResidence = 1,
      AlternateResidence = 2,
      Shipping = 3,
      Billing = 4
   }

   public class PhysicalAddress : Record<PhysicalAddress>
   {
      public string Line1 { get; }
      public string Line2 { get; }
      public string City { get; }
      public string State { get; }  
      public string Country { get; }
      public string Zip { get; }

      public PhysicalAddress(string line1, string line2, string city, string state, string country, string zip)
      {
         Line1 = ThrowIfEmpty(line1, "line1");
         Line2 = Trim(line2);
         City = ThrowIfEmpty(city, "city");
         State = Trim(state);
         Country = Trim(country);
         Zip = Trim(zip);
      }
   }
}
