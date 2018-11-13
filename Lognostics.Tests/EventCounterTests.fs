module EventCounterTests

open FsCheck
open FsCheck.Xunit
open Lognostics
open Swensen.Unquote
open System

[<Property>]
let ``test gives correct event count`` () =
    let eventCounter = EventCounter()
    let eventCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll eventCounts (fun eventCount ->
        for _ in [ 0 .. eventCount - 1 ] do
            eventCounter.HandleEvent(obj(), EventArgs())
        test <@ eventCounter.EventCount = eventCount @>)
