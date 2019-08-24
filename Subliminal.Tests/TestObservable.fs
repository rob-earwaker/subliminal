namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
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
