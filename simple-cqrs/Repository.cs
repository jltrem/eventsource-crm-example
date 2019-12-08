using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SimpleCQRS
{
   public interface IRepository<T> where T : Aggregate
   {
      Either<string, Unit> Save(T aggregate);
      Either<string, (T, Seq<Event>)> Load(Guid rootId, Func<Seq<Event>, (T, Seq<Event>)> factory);
   }

   public static class RepositoryExt
   {
      public static Either<string, (T, Seq<Event>)> Load<T>(this IRepository<T> repo, ICommand cmd) where T : Aggregate =>
         repo.Load(cmd.RootId, x => Aggregate.Cons<T>(x));

      public static Either<string, (T, Seq<Event>)> LoadExpectedVersion<T>(this IRepository<T> repo, ICommand cmd, int expectedVersion) where T : Aggregate =>
         repo.Load(cmd.RootId, x => Aggregate.Cons<T>(x))
            .Bind(x => x.Item1.Info.Version == expectedVersion
               ? x
               : Left<string, (T, Seq<Event>)>("aggregate version is different"));
   }

   public sealed class Repository<T> : IRepository<T> where T : Aggregate
   {
      private readonly IEventStore _storage;

      public Repository(IEventStore storage)
      {
         _storage = storage;
      }

      public Either<string, (T, Seq<Event>)> Load(Guid rootId, Func<Seq<Event>, (T, Seq<Event>)> factory) =>
         _storage.GetEvents(rootId)
            .Map(factory);

      public Either<string, Unit> Save(T aggregate) =>
         aggregate
            .NewEvents
            .Map(x => new Event(x.AggregateInfo, x.Timestamp, x.Owner, x.EventData))
            .Map(x => _storage.AddEvent(x))
            .Sequence()
            .Match(
               Right: _ => _storage.Save(),
               Left: err => err);
   }
}
