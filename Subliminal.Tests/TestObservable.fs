namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive.Linq

module ``Test IObservable<TValue>`` =
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
