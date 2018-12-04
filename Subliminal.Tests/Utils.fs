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

let createDelegateFrom (eventHandler: IEventHandler<'TEventArgs>) =
    EventHandler<'TEventArgs>(fun sender eventArgs -> eventHandler.HandleEvent(sender, eventArgs))
