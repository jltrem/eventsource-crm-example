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

      public PhoneNumber(PhoneType type, string number, string ext)
      {
         PhoneType = type; // TODO: validate range
         Number = ThrowIfEmpty(number, "number");
         Ext = Trim(ext);        
      }
   }
}
