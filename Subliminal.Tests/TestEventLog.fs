module Subliminal.Tests.TestEventLog

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System.Reactive
open Xunit

[<Property>]
let ``emits context values`` (context1: obj) (context2: obj) =
    let eventLog = EventLog<obj>()
    let observer = TestObserver()
    use subscription = eventLog.Subscribe(observer)
    eventLog.LogOccurrence(context1)
    eventLog.LogOccurrence(context2)
    test <@ observer.ObservedValues = [ context1; context2 ] @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``can log occurrence without context`` () =
    let eventLog = EventLog()
    let observer = TestObserver()
    use subscription = eventLog.Subscribe(observer)
    eventLog.LogOccurrence()
    eventLog.LogOccurrence()
    test <@ observer.ObservedValues = [ Unit.Default; Unit.Default ] @>
    test <@ not observer.ObservableCompleted @>
