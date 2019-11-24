using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using LanguageExt;

namespace SimpleCQRS
{
   public interface ICommandBus
   {
      Unit Send(ICommand cmd);
      IDisposable Subscribe(Action<ICommand> onNext);
      IDisposable Subscribe<T>(Action<T> onNext) where T : class, ICommand;
   }

   public class CommandBus : ICommandBus
   {
      private readonly Subject<ICommand> _commands = new Subject<ICommand>();

      public Unit Send(ICommand cmd)
      {
         _commands.OnNext(cmd);
         return Unit.Default;
      }

      public IDisposable Subscribe(Action<ICommand> onNext) =>
         _commands.Subscribe(onNext);

      public IDisposable Subscribe<T>(Action<T> onNext) where T : class, ICommand =>
         _commands.Where(x => x is T).Subscribe(x => onNext((T)x));
   }

   public class CommandResult : Record<CommandResult>
   {
      public Guid CommandId { get; }
      public Either<string, Unit> Value { get; }

      public CommandResult(ICommand cmd, Either<string, Unit> value)
      {
         CommandId = cmd.CommandId;
         Value = value;
      }
   }

   public class CommandResult<T> : Record<CommandResult<T>>
   {
      public Guid CommandId { get; }
      public T Value { get; }

      public CommandResult(ICommand cmd, T value)
      {
         CommandId = cmd.CommandId;
         Value = value;
      }
   }

   public class ReadCommandResult<T> : Record<CommandResult<T>>
   {
      public Guid CommandId { get; }
      public Either<string, (T, Seq<Event>)> Value { get; }

      public ReadCommandResult(ICommand cmd, Either<string, (T, Seq<Event>)> value)
      {
         CommandId = cmd.CommandId;
         Value = value;
      }
   }
}
