namespace Fescq

open System

type EventStore () =
   interface IEventStore with
      
      member x.GetEvents aggregateId : Result<Event list, string> = 
         Error "foo"

      member x.AddEvent e : Result<unit, string> = 
         Error "foo"

      member x.Save () : Result<unit, string> = 
         Error "foo"




module EventStoreCSharp =

   let GetEvents (eventStore:IEventStore, aggId:Guid) =       
      eventStore.GetEvents aggId
      |> function
         | Ok events -> struct (Some events, None)
         | Error msg -> struct (None, Some msg)

   let AddEvent (eventStore:IEventStore, event:Event) =       
      eventStore.AddEvent event
      |> function
         | Ok ok -> struct (Some ok, None)
         | Error msg -> struct (None, Some msg)
   
   let Save (eventStore:IEventStore) = 
      eventStore.Save ()
      |> function
         | Ok ok -> struct (Some ok, None)
         | Error msg -> struct (None, Some msg)

