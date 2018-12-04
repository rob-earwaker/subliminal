module AggregateEventHandlerTests
   
open Subliminal
open Swensen.Unquote
open System
open Xunit

[<Fact>]
let ``test calls child event handlers once for each event handled`` () =
    let eventCounter1 = EventCounter()
    let eventCounter2 = EventCounter()
    let aggregateEventHandler = AggregateEventHandler(eventCounter1, eventCounter2)
    aggregateEventHandler.HandleEvent(obj(), EventArgs())
    test <@ eventCounter1.EventCount = 1 @>
    test <@ eventCounter2.EventCount = 1 @>
