module Subliminal.Tests.TestCounter

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``increment is observed`` (increment: obj) =
    let counter = Counter<obj>()
    let observer = TestObserver()
    use subscription = counter.Subscribe(observer)
    counter.IncrementBy(increment)
    test <@ not observer.ObservableCompleted @>
    test <@ observer.ObservedValues = [ increment ] @>