module Subliminal.Tests.TestEvent

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System.Reactive
open Xunit

[<Property>]
let ``completes after being raised`` (eventValue: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(eventValue)
    test <@ observer.ObservedValues = [ eventValue ] @>
    test <@ observer.ObservableCompleted @>

[<Property>]
let ``only emits first event value`` (eventValue1: obj) (eventValue2: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(eventValue1)
    event.Raise(eventValue2)
    test <@ observer.ObservedValues = [ eventValue1 ] @>
    test <@ observer.ObservableCompleted @>
    
[<Fact>]
let ``can be raised without event value`` () =
    let event = Event()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise()
    test <@ observer.ObservedValues = [ Unit.Default ] @>
    test <@ observer.ObservableCompleted @>
