using System;
using SimpleCQRS;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CRM.Domain
{
   public sealed class ContactCommandHandlers : CommandHandlers<Contact>, IDisposable
   {
      public override string Name { get; }

      private readonly Seq<IDisposable> _subscriptions;

      public ContactCommandHandlers(IUseRepo<Contact> useRepo, ICommandBus bus, ITimeService timeService, ISecurityPrincipalService principalService)
         : base(useRepo, timeService.UtcNow, principalService.ActiveUserName)
      {
         Name = ContactEvent.AggregateName;

         _subscriptions =
            Seq(new[]
            {
               bus.Subscribe<ReadContact>(Handle),
               bus.Subscribe<CreateContact>(Handle),
               bus.Subscribe<RenameContact>(Handle),
               bus.Subscribe<AddContactPhone>(Handle),
               bus.Subscribe<UpdateContactPhone>(Handle),
            });
      }

      public void Handle(ReadContact cmd) =>
         UseRepo(repo =>
            repo.Load(cmd)
               .Apply(x => new ReadCommandResult<Contact>(cmd, x))
               .Apply(cmd.Result.Set));

      public void Handle(CreateContact cmd) =>
         UseRepo(repo => 
            new ContactCreated(cmd.Name)
               .Apply(x => new Event(Info(cmd, 1), UtcNow(), Owner(), x))
               .Apply(x => new Contact(Aggregate.NoEvents, x))
               .Apply(repo.Save)
               .Apply(x => new CommandResult(cmd, x))
               .Apply(cmd.Result.Set));

      public void Handle(RenameContact cmd) =>
         Update(cmd, () => new ContactRenamed(cmd.Name));

      public void Handle(AddContactPhone cmd) =>
         Update(cmd, () => new ContactPhoneAdded(cmd.DetailId, cmd.Phone));

      public void Handle(UpdateContactPhone cmd) =>
         Update(cmd, () => new ContactPhoneUpdated(cmd.DetailId, cmd.Phone));

      private void Update(Command cmd, Func<IEventData> newEventData) =>
         UseRepo(repo =>
            repo.LoadExpectedVersion(cmd, cmd.OriginalVersion)
               .Bind(aggregate =>
               {
                  (var contact, var history) = aggregate;
                  return newEventData()
                     .Apply(x => new Event(Info(cmd, contact.Info.Version + 1), UtcNow(), Owner(), x))
                     .Apply(x => new Contact(history, x))
                     .Apply(repo.Save);
               })
               .Apply(x => new CommandResult(cmd, x))
               .Apply(cmd.Result.Set));

      public void Dispose() =>
         _subscriptions.Iter(x => x.Dispose());
   }
}
