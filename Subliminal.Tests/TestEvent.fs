module Subliminal.Tests.TestEvent

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System.Reactive
open Xunit

[<Property>]
let ``completes after being raised`` (event: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(event)
    test <@ observer.ObservedValues = [ event ] @>
    test <@ observer.ObservableCompleted @>

[<Property>]
let ``only emits first event value`` (event1: obj) (event2: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(event1)
    event.Raise(event2)
    test <@ observer.ObservedValues = [ event1 ] @>
    test <@ observer.ObservableCompleted @>
    
[<Fact>]
let ``can be raised without event value`` () =
    let event = Event()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise()
    test <@ observer.ObservedValues = [ Unit.Default ] @>
    test <@ observer.ObservableCompleted @>
