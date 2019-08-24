namespace Subliminal.Tests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive.Linq

module ``Test IObservable<TValue>`` =
    [<Property>]
    let ``delta combines values pairwise`` (value1: obj) (value2: obj) (value3: obj) =
        let observable = [ value1; value2; value3 ].ToObservable().Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].PreviousValue = value1 @>
        test <@ observer.ObservedValues.[0].CurrentValue = value2 @>
        test <@ observer.ObservedValues.[1].PreviousValue = value2 @>
        test <@ observer.ObservedValues.[1].CurrentValue = value3 @>

    [<Property>]
    let ``delta uses custom selector result`` (value1: obj) (value2: obj) (value3: obj) (result: obj) =
        let observable = [ value1; value2; value3 ].ToObservable().Delta(fun delta -> result)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues = [ result; result ] @>
        
    [<Property>]
    let ``rate returns intervals between values`` (value1: obj) (value2: obj) (value3: obj) =
        let observable = [ value1; value2; value3 ].ToObservable().Rate()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 3 @>
        test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[2].Interval > TimeSpan.Zero @>
        
    [<Property>]
    let ``rate returns original values as deltas`` (value1: obj) (value2: obj) (value3: obj) =
        let observable = [ value1; value2; value3 ].ToObservable().Rate()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 3 @>
        test <@ observer.ObservedValues.[0].Delta = value1 @>
        test <@ observer.ObservedValues.[1].Delta = value2 @>
        test <@ observer.ObservedValues.[2].Delta = value3 @>
        
    [<Property>]
    let ``rate of change returns intervals between pairwise values`` (value1: obj) (value2: obj) (value3: obj) =
        let observable = [ value1; value2; value3 ].ToObservable().RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
        
    [<Property>]
    let ``rate of change combines values pairwise`` (value1: obj) (value2: obj) (value3: obj) =
        let observable = [ value1; value2; value3 ].ToObservable().RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta.PreviousValue = value1 @>
        test <@ observer.ObservedValues.[0].Delta.CurrentValue = value2 @>
        test <@ observer.ObservedValues.[1].Delta.PreviousValue = value2 @>
        test <@ observer.ObservedValues.[1].Delta.CurrentValue = value3 @>
        
    [<Property>]
    let ``rate of change uses custom selector result`` (value1: obj) (value2: obj) (value3: obj) (result: obj) =
        let observable = [ value1; value2; value3 ].ToObservable().RateOfChange(fun delta -> result)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = result @>
        test <@ observer.ObservedValues.[1].Delta = result @>
                
module ``Test IObservable<int>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let observable = [ value1; value2; value3 ].ToObservable().Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
    
    [<Property>]
    let ``rate of change subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let observable = [ value1; value2; value3 ].ToObservable().RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
        test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
        
module ``Test IObservable<long>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let observable = [ value1; value2; value3 ].ToObservable().Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
    [<Property>]
    let ``rate of change subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let observable = [ value1; value2; value3 ].ToObservable().RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
        test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
        
module ``Test IObservable<TimeSpan>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` () =
        let test (value1: TimeSpan, value2: TimeSpan, value3: TimeSpan) =
            let observable = [ value1; value2; value3 ].ToObservable().Delta()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0] = value2 - value1 @>
            test <@ observer.ObservedValues.[1] = value3 - value2 @>
        let arb =
            Arb.generate<int64>
            |> Gen.map TimeSpan.FromTicks
            |> Gen.three
            |> Arb.fromGen
        Prop.forAll arb test
        
    [<Property>]
    let ``rate of change subtracts pairwise values`` () =
        let test (value1: TimeSpan, value2: TimeSpan, value3: TimeSpan) =
            let observable = [ value1; value2; value3 ].ToObservable().RateOfChange()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
            test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
        let arb =
            Arb.generate<int64>
            |> Gen.map TimeSpan.FromTicks
            |> Gen.three
            |> Arb.fromGen
        Prop.forAll arb test
                