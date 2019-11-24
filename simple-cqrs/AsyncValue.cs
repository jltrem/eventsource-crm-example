using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using LanguageExt;
using System.Threading.Tasks;

namespace SimpleCQRS
{
   public class AsyncValue<T>
   {
      private readonly ReplaySubject<T> _subject = new ReplaySubject<T>();

      public Unit Set(T value)
      {
         _subject.OnNext(value);
         _subject.OnCompleted();
         return Unit.Default;
      }

      public async Task<T> Wait() =>
         await _subject.LastAsync();
   }
}
