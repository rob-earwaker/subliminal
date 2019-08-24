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
        
    [<Property>]
    let ``observers receive the same events`` (event1: obj) (event2: obj) =
        let eventLog = EventLog<obj>()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = eventLog.Subscribe(observer1)
        use subscription2 = eventLog.Subscribe(observer2)
        eventLog.LogOccurrence(event1)
        eventLog.LogOccurrence(event2)
        test <@ observer1.ObservedValues = [ event1; event2 ] @>
        test <@ observer2.ObservedValues = [ event1; event2 ] @>
        
    [<Property>]
    let ``event counter increments by one per event`` (event1: obj) (event2: obj) =
        let eventLog = EventLog<obj>()
        let observer = TestObserver()
        use subscription = eventLog.EventCounter.Subscribe(observer)
        eventLog.LogOccurrence(event1)
        eventLog.LogOccurrence(event2)
        test <@ observer.ObservedValues = [ 1L; 1L ] @>
    
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
        
    [<Property>]
    let ``all observers receive the events`` () =
        let eventLog = EventLog()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = eventLog.Subscribe(observer1)
        use subscription2 = eventLog.Subscribe(observer2)
        eventLog.LogOccurrence()
        eventLog.LogOccurrence()
        test <@ observer1.ObservedValues = [ Unit.Default; Unit.Default ] @>
        test <@ observer2.ObservedValues = [ Unit.Default; Unit.Default ] @>
        
    [<Property>]
    let ``event counter increments by one per event`` () =
        let eventLog = EventLog()
        let observer = TestObserver()
        use subscription = eventLog.EventCounter.Subscribe(observer)
        eventLog.LogOccurrence()
        eventLog.LogOccurrence()
        test <@ observer.ObservedValues = [ 1L; 1L ] @>
