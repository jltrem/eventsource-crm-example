using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SimpleCQRS
{
   public interface ICommand 
   {
      Guid CommandId { get; }
      Guid RootId { get; }
   }

   public abstract class ReadAggregate<T> : ICommand
   {
      public Guid CommandId { get; } = Guid.NewGuid();
      public Guid RootId { get; } = Guid.NewGuid();
      public AsyncValue<ReadCommandResult<T>> Result { get; } = new AsyncValue<ReadCommandResult<T>>();

      public ReadAggregate(Guid rootId) => 
         RootId = rootId;
   }

   public abstract class CreateRootCommand : ICommand
   {
      public Guid CommandId { get; } = Guid.NewGuid();
      public Guid RootId { get; } = Guid.NewGuid();
      public AsyncValue<CommandResult> Result { get; } = new AsyncValue<CommandResult>();
   }

   public abstract class Command : ICommand
   {
      public Guid CommandId { get; } = Guid.NewGuid();
      public Guid RootId { get; }
      public AsyncValue<CommandResult> Result { get; } = new AsyncValue<CommandResult>();
      public int OriginalVersion { get; }

      public Command(Guid rootId, int originalVersion)
      {
         RootId = rootId;
         OriginalVersion = originalVersion < 1
            ? throw new ArgumentException("originalVersion must be at least 1")
            : originalVersion;
      }
   }

   public abstract class DetailCommand : Command
   {
      public Guid DetailId { get; }

      public DetailCommand(Guid rootId, int originalVersion)
         : base(rootId, originalVersion)
      {
         DetailId = Guid.NewGuid();
      }

      public DetailCommand(Guid rootId, Guid detailId, int originalVersion)
         : base(rootId, originalVersion)
      {
         DetailId = detailId;
      }
   }
}
