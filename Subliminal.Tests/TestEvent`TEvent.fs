module Subliminal.Tests.``Test Event<TEvent>``

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``emits value when raised`` (value: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(value)
    test <@ observer.ObservedValues = [ value ] @>

[<Property>]
let ``completes after being raised`` (value: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(value)
    test <@ observer.ObservedValues = [ value ] @>
    test <@ observer.ObservableCompleted @>

[<Property>]
let ``only emits first value`` (value1: obj) (value2: obj) =
    let event = Event<obj>()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise(value1)
    event.Raise(value2)
    test <@ observer.ObservedValues = [ value1 ] @>
