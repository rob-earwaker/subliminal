module AggregateEventHandlerTests
   
open FsCheck
open FsCheck.Xunit
open Lognostics
open Swensen.Unquote
open System

[<Property>]
let ``test calls child event handlers once for each event handled`` () =
    let eventHandlerCounts = Gen.choose (0, 10) |> Arb.fromGen
    Prop.forAll eventHandlerCounts (fun eventHandlerCount ->
        // Arrange
        let eventCounters = Array.init eventHandlerCount (fun _ -> EventCounter())
        let eventHandlers = eventCounters |> Array.map (fun eventCounter -> eventCounter :> IEventHandler<_>)
        let aggregateEventHandler = AggregateEventHandler(eventHandlers)
        // Act
        aggregateEventHandler.HandleEvent(obj(), EventArgs())
        // Assert
        for eventCounter in eventCounters do
            test <@ eventCounter.EventCount = 1 @>)
