﻿namespace Subliminal.Tests

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
                
module ``Test IObservable<int>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let observable = [ value1; value2; value3 ].ToObservable().Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
module ``Test IObservable<long>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let observable = [ value1; value2; value3 ].ToObservable().Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
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
                