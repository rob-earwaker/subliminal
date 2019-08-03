module Subliminal.Tests.TestEvent

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``completes after being raised`` (context: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(context)
    test <@ observer.ObservedValues = [ context ] @>
    test <@ observer.ObservableCompleted @>

[<Property>]
let ``only emits first context value`` (context1: obj) (context2: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(context1)
    event.Raise(context2)
    test <@ observer.ObservedValues = [ context1 ] @>
    test <@ observer.ObservableCompleted @>