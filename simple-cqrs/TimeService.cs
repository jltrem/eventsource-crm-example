using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleCQRS
{
   public interface ITimeService
   {
      Func<DateTimeOffset> UtcNow { get; }
   }

   public class TimeService : ITimeService
   {
      public Func<DateTimeOffset> UtcNow { get; }

      public TimeService(Func<DateTimeOffset> utcNow) =>
         UtcNow = utcNow;
   }
}
