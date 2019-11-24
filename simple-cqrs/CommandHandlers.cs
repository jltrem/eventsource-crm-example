using System;

namespace SimpleCQRS
{
   public abstract class CommandHandlers<T> where T : Aggregate
   {
      protected readonly Action<Action<IRepository<T>>> UseRepo;
      protected readonly Func<DateTimeOffset> UtcNow;
      protected readonly Func<string> Owner;

      public abstract string Name { get; }

      protected CommandHandlers(IUseRepo<T> useRepo, Func<DateTimeOffset> utcNow, Func<string> owner)
      {
         if (useRepo == null) throw new ArgumentNullException("useRepo");
         if (utcNow == null) throw new ArgumentNullException("utcNow");
         if (owner == null) throw new ArgumentNullException("owner");

         UseRepo = useRepo.UseRepo;
         UtcNow = utcNow;
         Owner = owner;
      }

      protected AggregateInfo Info(ICommand cmd, int version) =>
         new AggregateInfo(Name, cmd.RootId, version);
   }
}
