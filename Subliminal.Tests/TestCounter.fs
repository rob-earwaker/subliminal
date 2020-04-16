namespace Subliminal.Tests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System

module ``Test Counter`` =
    [<Property>]
    let ``emits increments when incremented`` (NormalFloat increment1) (NormalFloat increment2) =
        let counter = Counter()
        let observer = TestObserver()
        use subscription = counter.Subscribe(observer)
        counter.IncrementBy(increment1)
        counter.IncrementBy(increment2)
        test <@ observer.ObservedValues = [ increment1; increment2 ] @>
        test <@ not observer.ObservableCompleted @>

    [<Property>]
    let ``emits increment of one when incremented`` () =
        let counter = Counter()
        let observer = TestObserver()
        use subscription = counter.Subscribe(observer)
        counter.Increment()
        test <@ observer.ObservedValues = [ 1.0 ] @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``observers receive the same increments`` (NormalFloat increment1) (NormalFloat increment2) =
        let counter = Counter()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = counter.Subscribe(observer1)
        use subscription2 = counter.Subscribe(observer2)
        counter.IncrementBy(increment1)
        counter.IncrementBy(increment2)
        test <@ observer1.ObservedValues = [ increment1; increment2 ] @>
        test <@ observer2.ObservedValues = [ increment1; increment2 ] @>
        
    [<Property>]
    let ``rate returns original values as deltas`` (NormalFloat value1) (NormalFloat value2) (NormalFloat value3) =
        let counter = Counter()
        let observable = counter.Rate()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        counter.IncrementBy(value1)
        counter.IncrementBy(value2)
        counter.IncrementBy(value3)
        test <@ observer.ObservedValues.Length = 3 @>
        test <@ observer.ObservedValues.[0].Delta = value1 @>
        test <@ observer.ObservedValues.[1].Delta = value2 @>
        test <@ observer.ObservedValues.[2].Delta = value3 @>

    [<Property>]
    let ``rate returns intervals between values`` (NormalFloat value1) (NormalFloat value2) (NormalFloat value3) =
        let counter = Counter()
        let observable = counter.Rate()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        counter.IncrementBy(value1)
        counter.IncrementBy(value2)
        counter.IncrementBy(value3)
        test <@ observer.ObservedValues.Length = 3 @>
        test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[2].Interval > TimeSpan.Zero @>
