namespace Fescq.Core

type Agg<'entity> = {
   Key: AggregateKey
   Entity: 'entity
   NewEvents: Event list
}

module Aggregate = 

   /// Apply all events with the provided folder function.
   /// This function must validate domain rules so that construction 
   /// is successful IFF the state is valid
   let create<'entity> (apply:(int*'entity) -> Event -> (int*'entity)) (empty:'entity) (history:Event list) (future:Event list)  =

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

      let entity = 
         events
         |> List.fold (fun s e -> 
               let version = (fst s) + 1
               let entity = (snd s)

               apply (version, entity) e
            ) (0,empty)
         |> snd

      { Key = key
        Entity = entity
        NewEvents = future }

   let createWithNewEvent<'entity> (apply:(int*'entity) -> Event -> (int*'entity)) (empty:'entity) (history:Event list) (newEvent:Event) =       
      create apply empty history [newEvent]

   let createFromHistory<'entity> (apply:(int*'entity) -> Event -> (int*'entity)) (empty:'entity) (history:Event list) =
      create apply empty history []

