using System;
using System.Collections.Generic;
using CRM.Domain.Types;
using SimpleCQRS;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CRM.Domain.DTO
{
   public class Aggregate<T>
   {
      public AggregateInfo Info { get; set; }
      public T Data { get; set; }

      public Aggregate(AggregateInfo info, T data)
      {
         Info = info;
         Data = data;
      }
   }
}
