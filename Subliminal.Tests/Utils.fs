module Utils

open Subliminal
open System
open System.Collections.Generic

type EventCollector<'TEventArgs when 'TEventArgs :> EventArgs>() =
    let receivedEvents = List<'TEventArgs>()

    interface IEventHandler<'TEventArgs> with
        member __.HandleEvent(sender, eventArgs) =
            receivedEvents.Add(eventArgs)

    member val ReceivedEvents = receivedEvents

type ValueCollector<'TValue>() =
    let receivedValues = List<'TValue>()

    interface IObserver<'TValue> with
        member __.OnNext(value) =
            receivedValues.Add(value)
        
        member __.OnCompleted() =
            ()

        member __.OnError(exn) =
            ()

    member val ReceivedValues = receivedValues

let createDelegateFrom (eventHandler: IEventHandler<'TEventArgs>) =
    EventHandler<'TEventArgs>(fun sender eventArgs -> eventHandler.HandleEvent(sender, eventArgs))
