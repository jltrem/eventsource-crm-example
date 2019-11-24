using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SimpleCQRS
{
   public abstract class Aggregate
   {
      public AggregateInfo Info { get; }

      public Seq<Event> NewEvents { get; }

      public static Seq<Event> NoEvents => Lst<Event>.Empty.ToSeq();
      public static Seq<Event> OneEvent(Event e) => Seq(new[] { e });

      /// <summary>
      /// Call the constructor for the specified aggregate class, with the event history,
      /// and return a tuple of (aggregate, history)
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="history"></param>
      /// <returns></returns>
      public static (T aggregate, Seq<Event> history) Cons<T>(Seq<Event> history) where T : Aggregate =>
         typeof(T)
            .GetConstructor(new[] { typeof(Seq<Event>) })
            .Invoke(new object[] { history })
            .Apply(x => ((T)x, history));

      /// <summary>
      /// Ctor used when loading an existing history plus new events that are not yet persisted.
      /// Create an aggregate with the specified id and event history.
      /// If the event would put the aggregate into an invalid state, then "apply" should throw an exception.
      /// </summary>
      /// <param name="id"></param>
      /// <param name="history"></param>
      /// <param name="apply"></param>
      protected Aggregate(Seq<Event> history, Seq<Event> future, Action<Aggregate, int, Event> apply)
      {
         if (history == null) throw new ArgumentNullException("history");
         if (future == null) throw new ArgumentNullException("future");

         var events = history.Append(future);
         if (events.Count == 0) throw new ArgumentNullException("events cannot be empty");
         if (events.Count >= 2)
         {
            events.Aggregate((prev, next) => prev.AggregateInfo.RootId == next.AggregateInfo.RootId
               ? next
               : throw new ArgumentException("events must refer to the same root id"));
         }

         Info = events.Last().AggregateInfo;
         NewEvents = future;

         var _ = events.Iter((i, e) => apply(this, i + 1, e));
      }

      /// <summary>
      /// Ctor used when loading an existing history plus a single new event that is not yet persisted.
      /// </summary>
      /// <param name="history"></param>
      /// <param name="newEvent"></param>
      /// <param name="apply"></param>
      protected Aggregate(Seq<Event> history, Event newEvent, Action<Aggregate, int, Event> apply)
         : this(history, OneEvent(newEvent), apply)
      {
      }

      /// <summary>
      /// Ctor used when loading an existing history
      /// </summary>
      /// <param name="history"></param>
      /// <param name="apply"></param>
      protected Aggregate(Seq<Event> history, Action<Aggregate, int, Event> apply)
         : this(history, NoEvents, apply)
      {
      }
   }
}
