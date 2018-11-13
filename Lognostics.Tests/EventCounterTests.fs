module EventCounterTests

open FsCheck
open FsCheck.Xunit
open Lognostics
open Swensen.Unquote
open System

[<Property>]
let ``test gives correct event count`` () =
    let eventCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll eventCounts (fun eventCount ->
        // Arrange
        let eventCounter = EventCounter()
        // Act
        for _ in [ 0 .. eventCount - 1 ] do
            eventCounter.HandleEvent(obj(), EventArgs())
        // Assert
        test <@ eventCounter.EventCount = eventCount @>)
