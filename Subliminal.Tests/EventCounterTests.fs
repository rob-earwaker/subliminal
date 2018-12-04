module EventCounterTests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System

[<Property>]
let ``test gives correct event count`` () =
    let eventCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll eventCounts (fun eventCount ->
        let eventCounter = EventCounter()
        for _ in [ 0 .. eventCount - 1 ] do
            eventCounter.HandleEvent(obj(), EventArgs())
        test <@ eventCounter.EventCount = eventCount @>)
