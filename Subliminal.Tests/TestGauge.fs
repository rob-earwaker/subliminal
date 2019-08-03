module Subliminal.Tests.TestGauge

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``emits recorded values`` (value1: obj) (value2: obj) =
    let gauge = Gauge<obj>()
    let observer = TestObserver()
    use subscription = gauge.Subscribe(observer)
    gauge.LogValue(value1)
    gauge.LogValue(value2)
    test <@ observer.ObservedValues = [ value1; value2 ] @>
    test <@ not observer.ObservableCompleted @>
