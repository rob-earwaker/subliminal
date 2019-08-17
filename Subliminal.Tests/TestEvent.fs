module Subliminal.Tests.``Test Event``

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System.Reactive

[<Property>]
let ``emits unit when raised`` () =
    let event = Event()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise()
    test <@ observer.ObservedValues = [ Unit.Default ] @>

[<Property>]
let ``completes after being raised`` () =
    let event = Event()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise()
    test <@ observer.ObservedValues = [ Unit.Default ] @>
    test <@ observer.ObservableCompleted @>

[<Property>]
let ``only emits one value`` () =
    let event = Event()
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    event.Raise()
    event.Raise()
    test <@ observer.ObservedValues = [ Unit.Default ] @>
