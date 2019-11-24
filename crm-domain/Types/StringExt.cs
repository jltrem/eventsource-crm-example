using System;
using LanguageExt;

namespace CRM.Domain.Types
{
   internal static class StringExt
   {
      public static string Trim(string value) =>
         (value ?? "").Trim();

      public static string ThrowIfEmpty(string value, string fieldName) =>
         Trim(value)
            .Apply(x => string.IsNullOrEmpty(x)
               ? throw new ArgumentException($"{fieldName} cannot be empty")
               : x);
   }
}
