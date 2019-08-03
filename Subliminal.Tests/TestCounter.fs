module Subliminal.Tests.TestCounter

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``emits increments`` (increment1: obj) (increment2: obj) =
    let counter = Counter<obj>()
    let observer = TestObserver()
    use subscription = counter.Subscribe(observer)
    counter.IncrementBy(increment1)
    counter.IncrementBy(increment2)
    test <@ observer.ObservedValues = [ increment1; increment2 ] @>
    test <@ not observer.ObservableCompleted @>