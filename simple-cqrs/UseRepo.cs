using System;

namespace SimpleCQRS
{
   public interface IUseRepo<T> where T : Aggregate
   {
      void UseRepo(Action<IRepository<T>> action);
   }
}
