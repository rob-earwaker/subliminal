module GaugeTests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open Utils

[<Property>]
let ``test includes gauge value in gauge sampled event args`` (gaugeValue: obj) =
    let gauge = Gauge()
    let observationCollector = ValueCollector()
    use subscription = gauge.Sampled.Subscribe(observationCollector)
    gauge.LogValue(gaugeValue)
    test <@ observationCollector.ReceivedValues.Count = 1 @>
    test <@ observationCollector.ReceivedValues.[0] = gaugeValue @>
