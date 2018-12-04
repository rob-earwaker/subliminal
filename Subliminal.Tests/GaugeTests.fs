module GaugeTests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open Utils

[<Property>]
let ``test includes gauge value in gauge sampled event args`` (gaugeValue: obj) =
    let gauge = Gauge()
    let eventCollector = EventCollector()
    gauge.Sampled.AddHandler(createDelegateFrom eventCollector)
    gauge.LogValue(gaugeValue)
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].Value = gaugeValue @>
