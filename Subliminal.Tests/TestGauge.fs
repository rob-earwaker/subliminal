namespace Subliminal.Tests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System

module ``Test Gauge`` =
    [<Property>]
    let ``emits values when logged`` (NormalFloat value1) (NormalFloat value2) =
        let gauge = Gauge()
        let observer = TestObserver()
        use subscription = gauge.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        test <@ observer.ObservedValues = [ value1; value2 ] @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``observers receive the same values`` (NormalFloat value1) (NormalFloat value2) =
        let gauge = Gauge()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = gauge.Subscribe(observer1)
        use subscription2 = gauge.Subscribe(observer2)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        test <@ observer1.ObservedValues = [ value1; value2 ] @>
        test <@ observer2.ObservedValues = [ value1; value2 ] @>
        
    [<Property>]
    let ``delta subtracts values pairwise`` (NormalFloat value1) (NormalFloat value2) (NormalFloat value3) =
        let gauge = Gauge()
        let observable = gauge.Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
    [<Property>]
    let ``rate of change returns intervals between pairwise values`` (NormalFloat value1) (NormalFloat value2) (NormalFloat value3) =
        let gauge = Gauge()
        let observable = gauge.RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
        
    [<Property>]
    let ``rate of change subtracts values pairwise`` (NormalFloat value1) (NormalFloat value2) (NormalFloat value3) =
        let gauge = Gauge()
        let observable = gauge.RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
        test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
