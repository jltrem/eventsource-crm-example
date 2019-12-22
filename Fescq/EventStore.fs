namespace Fescq.Core

open System

type IEventStore =
   abstract member GetEvents : Guid -> Result<Event list, string>
   abstract member AddEvent : Event -> Result<unit, string>
   abstract member Save : unit -> Result<unit, string>
