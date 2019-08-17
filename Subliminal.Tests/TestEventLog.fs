namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System.Reactive

module ``Test EventLog<TEvent>`` =
    [<Property>]
    let ``emits events when occurrences logged`` (event1: obj) (event2: obj) =
        let eventLog = EventLog<obj>()
        let observer = TestObserver()
        use subscription = eventLog.Subscribe(observer)
        eventLog.LogOccurrence(event1)
        eventLog.LogOccurrence(event2)
        test <@ observer.ObservedValues = [ event1; event2 ] @>
        test <@ not observer.ObservableCompleted @>
    
module ``Test EventLog`` =
    [<Property>]
    let ``emits events when occurrences logged`` () =
        let eventLog = EventLog()
        let observer = TestObserver()
        use subscription = eventLog.Subscribe(observer)
        eventLog.LogOccurrence()
        eventLog.LogOccurrence()
        test <@ observer.ObservedValues = [ Unit.Default; Unit.Default ] @>
        test <@ not observer.ObservableCompleted @>
