using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static CRM.Domain.Types.StringExt;

namespace CRM.Domain.Types
{
   public class CompanyName : Record<CompanyName>
   {
      public string Name { get; }

      public CompanyName(string name) =>
         Name = Validate(name);

      private static string Validate(string value) =>
         Trim(value)
            .Apply(x => string.IsNullOrEmpty(x)
               ? throw new ArgumentException("name cannot be empty")
               : x);
   }
}
