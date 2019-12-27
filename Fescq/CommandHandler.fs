namespace Fescq

open System
open Fescq.Command


[<AbstractClass>]
type CommandHandler<'t> (useRepo:Action<Action<IRepository<'t>>>, utcNow:Func<DateTimeOffset>, activeUserName:Func<string>) = 

   member val UseRepo = useRepo
   member val UtcNow = utcNow
   member val Owner = activeUserName

   abstract Name: string with get

   member x.Key (cmd:ICommand, version) =
      { Name = x.Name
        Id = cmd.AggregateId
        Version = version }


// These abstractions can be used for DI
type IRepositoryProvider<'t> =
   abstract member UseRepo : Action<Action<IRepository<'t>>>

type ITimeProvider =
   abstract member UtcNow : Func<DateTimeOffset>

type ISecurityPrincipalProvider =
   abstract member ActiveUserName : Func<string>


