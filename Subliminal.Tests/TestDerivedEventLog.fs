module Subliminal.Tests.TestDerivedEventLog

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects

type DerivedEventLogFactory =
    DerivedEventLogFactory of deriveEventLog:(IObservable<obj> -> IEventLog<obj>) with
    static member Arb =
        [ fun observable -> DerivedEventLog<obj>.FromObservable(observable) :> IEventLog<obj>
          fun observable -> observable.AsEventLog<obj>() ]
        |> Gen.elements
        |> Gen.map DerivedEventLogFactory
        |> Arb.fromGen

[<Property>]
let ``emits events`` (event1: obj) (event2: obj) =
    let test (DerivedEventLogFactory deriveEventLog) =
        use subject = new Subject<obj>()
        let eventLog = deriveEventLog subject
        let observer = TestObserver()
        use subscription = eventLog.Subscribe(observer)
        subject.OnNext(event1)
        subject.OnNext(event2)
        test <@ observer.ObservedValues = [ event1; event2 ] @>
    Prop.forAll DerivedEventLogFactory.Arb test
    
[<Property>]
let ``observers receive the same events`` () =
    let test (DerivedEventLogFactory deriveEventLog) =
        // Create an observable that emits two values after a
        // subject is completed. This is a cold observable and
        // will emit different values to each observer.
        use subject = new Subject<obj>()
        let observable =
            Observable.Concat(
                subject,
                Observable.Return(Unit.Default).Select(fun _ -> obj()),
                Observable.Return(Unit.Default).Select(fun _ -> obj()))
        let eventLog = deriveEventLog observable
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = eventLog.Subscribe(observer1)
        use subscription2 = eventLog.Subscribe(observer2)
        // Complete the subject to emit the values.
        subject.OnCompleted()
        // Both observers should have received the same values if the
        // cold observable was converted into a hot observable.
        test <@ observer1.ObservedValues.Length = 2 @>
        test <@ observer2.ObservedValues.Length = 2 @>
        test <@ observer1.ObservedValues = observer2.ObservedValues @>
    Prop.forAll DerivedEventLogFactory.Arb test
    
[<Property>]
let ``starts emitting events immediately`` (event1: obj) (event2: obj) =
    let test (DerivedEventLogFactory deriveEventLog) =
        // Create an observable that emits one value immediately
        // and a second value when a subject is completed.
        use subject = new Subject<obj>()
        let observable =
            Observable.Concat(
                Observable.Return(event1),
                subject,
                Observable.Return(event2))
        let eventLog = deriveEventLog observable
        let observer = TestObserver()
        use subscription = eventLog.Subscribe(observer)
        // The first value should have been emitted before the
        // observer was subscribed if the derived observable
        // started emitting values immediately.
        test <@ observer.ObservedValues = [] @>
        // Complete the subject to emit the second value.
        subject.OnCompleted()
        // The only observed value should be the second value.
        test <@ observer.ObservedValues = [ event2 ] @>
    Prop.forAll DerivedEventLogFactory.Arb test