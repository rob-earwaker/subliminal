module Subliminal.Tests.TestEventLog

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System.Reactive
open Xunit

[<Property>]
let ``emits events`` (eventValue1: obj) (eventValue2: obj) =
    let eventLog = EventLog<obj>()
    let observer = TestObserver()
    use subscription = eventLog.Subscribe(observer)
    eventLog.LogOccurrence(eventValue1)
    eventLog.LogOccurrence(eventValue2)
    test <@ observer.ObservedValues = [ eventValue1; eventValue2 ] @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``can log occurrence without event value`` () =
    let eventLog = EventLog()
    let observer = TestObserver()
    use subscription = eventLog.Subscribe(observer)
    eventLog.LogOccurrence()
    eventLog.LogOccurrence()
    test <@ observer.ObservedValues = [ Unit.Default; Unit.Default ] @>
    test <@ not observer.ObservableCompleted @>
