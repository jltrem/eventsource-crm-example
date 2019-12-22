namespace Fescq.Core

open System

type IRepository<'t> =

   abstract member Save : Agg<'t> -> Result<Agg<'t>, string>

   /// input: aggregate ID * factory
   /// output: aggregate * history
   abstract member Load : Guid * (Event list -> Result<Agg<'t>, string>) -> Result<(Agg<'t> * Event list), string>



type IUseRepository<'t> =
   abstract member UseRepo : Action<IRepository<'t>> -> unit


type Repository<'t> (storage:IEventStore) =

   interface IRepository<'t> with

      member x.Load (aggregateId:Guid, factory:(Event list -> Result<Agg<'t>, string>)) =
         aggregateId
         |> storage.GetEvents
         |> Result.bind (fun events ->
               events
               |> factory
               |> Result.bind (fun agg -> Ok (agg, events)
               )
            )

      member x.Save (aggregate:Agg<'t>) =
         if aggregate.NewEvents.Length > 0 then
            aggregate.NewEvents
            
            // bail at the first error
            // https://stackoverflow.com/a/26890974/571637
            |> List.unfold (fun events ->
                  match events with 
                  | head :: tail ->
                     match storage.AddEvent head with
                     | Ok _ -> Some (Ok (), tail)
                     | Error msg -> Some (Error msg, []) 
                  | [] -> None)

            |> List.last 
            |> Result.bind storage.Save
            |> Result.bind (fun _ -> { aggregate with NewEvents = [] } |> Ok)
         else
            aggregate |> Ok

(*

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


*)