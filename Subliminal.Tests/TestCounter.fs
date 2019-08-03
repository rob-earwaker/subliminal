module Subliminal.Tests.TestCounter

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``increment is observed`` increment =
    let counter = Counter<obj>()
    let observer = TestObserver()
    use subscription = counter.Subscribe(observer)
    counter.IncrementBy(increment)
    test <@ not observer.ObservableCompleted @>
    test <@ observer.ObservedValues.Count = 1 @>
    test <@ observer.ObservedValues.[0] = increment @>