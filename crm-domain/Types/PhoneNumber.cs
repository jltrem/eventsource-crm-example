using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static CRM.Domain.Types.StringExt;

namespace CRM.Domain.Types
{
   public enum PhoneType
   {
      Unknown = 0,
      Mobile = 1,
      Work = 2,
      Home = 3
   }

   public sealed class PhoneNumber : Record<PhoneNumber>
   {
      public PhoneType PhoneType { get; }
      public string Number { get; }
      public string Ext { get; }

      public PhoneNumber(string phoneType, string number, string ext)
      {
         PhoneType = Enum.TryParse(phoneType, true, out PhoneType type)
            ? type
            : PhoneType.Unknown;

         Number = ThrowIfEmpty(number, "number");
         Ext = Trim(ext);
      }

      [Newtonsoft.Json.JsonConstructor]
      public PhoneNumber(PhoneType phoneType, string number, string ext)
      {
         PhoneType = phoneType; // TODO: validate range
         Number = ThrowIfEmpty(number, "number");
         Ext = Trim(ext);        
      }
   }
}
