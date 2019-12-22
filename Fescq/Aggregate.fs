namespace Fescq.Core

module Aggregate = 

   let noEvents = List<Event>.Empty
   let oneEvent e = List<Event>.Cons(e, [])
     

[<AbstractClass>]
type Aggregate (history:Event list, future:Event list, apply:Aggregate -> int -> Event -> unit) as self = 

   let events = 
      history @ future
      |> function
         | [] -> failwith "events cannot be empty"
         | all -> 
            all
            |> List.map(fun x -> x.AggregateKey.Id)            
            |> List.distinct
            |> function
               | [_] -> all
               | _ -> failwith "events must refer to the same aggregate id"

   let key = 
      events 
      |> List.last
      |> fun x -> x.AggregateKey

   // Apply all events with the provided function.
   // This function must validate domain rules so that the 
   // construction is successful IFF the state is valid
   do 
      events 
      |> List.iteri (fun i e -> apply self (i + 1) e)   

   member x.Key : AggregateKey = key
   member x.NewEvents : Event list = future
   