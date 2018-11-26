module GaugeTests

open FsCheck.Xunit
open Lognostics
open Swensen.Unquote
open Utils
open Xunit

[<Fact>]
let ``test gauges have different gauge ids`` () =
    let gauge1 = Gauge()
    let gauge2 = Gauge()
    test <@ gauge1.GaugeId <> gauge2.GaugeId @>

[<Property>]
let ``test includes gauge value in gauge sampled event args`` (gaugeValue: obj) =
    let gauge = Gauge()
    let eventCollector = EventCollector()
    gauge.Sampled.AddHandler(createDelegateFrom eventCollector)
    gauge.LogValue(gaugeValue)
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].Value = gaugeValue @>

[<Property>]
let ``test includes gauge id in gauge sampled event args`` (gaugeValue: obj) =
    let gauge = Gauge()
    let eventCollector = EventCollector()
    gauge.Sampled.AddHandler(createDelegateFrom eventCollector)
    gauge.LogValue(gaugeValue)
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].GaugeId = gauge.GaugeId @>
