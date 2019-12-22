namespace Fescq.Core

open System;

/// This marks an event data class
type IEventData = interface end

/// All IEventData classes must be marked with this attribute which specifies revision
[<AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)>]
type EventDataAttribute(name:string, version:int) = 
   inherit Attribute()
   member x.Name with get() = name
   member x.Version with get() = version

type AggregateKey = {
   Name: string
   Id: Guid
   Version: int
}

type Event = {
   AggregateKey: AggregateKey
   Timestamp: DateTimeOffset
   MetaData: string
   EventData: IEventData
}
