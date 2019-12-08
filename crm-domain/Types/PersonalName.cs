using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static CRM.Domain.Types.StringExt;

namespace CRM.Domain.Types
{
   public sealed class PersonalName : Record<PersonalName>
   {
      public string Given { get; }
      public string Middle { get; }
      public string Family { get; }

      public PersonalName(string given, string middle, string family) =>
         (Given, Middle, Family) = Validate(given, middle, family);

      private static (string given, string middle, string family) Validate(string given, string middle, string family)
      {
         var g = Trim(given);
         var m = Trim(middle);
         var f = Trim(family);

         return string.Concat(g, m, f)
            .Apply(x => string.IsNullOrEmpty(x)
               ? throw new ArgumentException("name cannot be empty")
               : (g, m, f));
      }
   }
}
