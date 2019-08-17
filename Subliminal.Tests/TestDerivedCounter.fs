module Subliminal.Tests.TestDerivedCounter

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects

type DerivedCounterFactory =
    DerivedCounterFactory of deriveCounter:(IObservable<obj> -> ICounter<obj>) with
    static member Arb =
        [ fun observable -> DerivedCounter<obj>.FromObservable(observable) :> ICounter<obj>
          fun observable -> observable.AsCounter<obj>() ]
        |> Gen.elements
        |> Gen.map DerivedCounterFactory
        |> Arb.fromGen

[<Property>]
let ``emits increments`` (increment1: obj) (increment2: obj) =
    let test (DerivedCounterFactory deriveCounter) =
        use subject = new Subject<obj>()
        let counter = deriveCounter subject
        let observer = TestObserver()
        use subscription = counter.Subscribe(observer)
        subject.OnNext(increment1)
        subject.OnNext(increment2)
        test <@ observer.ObservedValues = [ increment1; increment2 ] @>
    Prop.forAll DerivedCounterFactory.Arb test
    
[<Property>]
let ``observers receive the same increments`` () =
    let test (DerivedCounterFactory deriveCounter) =
        // Create an observable that emits two values after a
        // subject is completed. This is a cold observable and
        // will emit different values to each observer.
        use subject = new Subject<obj>()
        let observable =
            Observable.Concat(
                subject,
                Observable.Return(Unit.Default).Select(fun _ -> obj()),
                Observable.Return(Unit.Default).Select(fun _ -> obj()))
        let counter = deriveCounter observable
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = counter.Subscribe(observer1)
        use subscription2 = counter.Subscribe(observer2)
        // Complete the subject to emit the values.
        subject.OnCompleted()
        // Both observers should have received the same values if the
        // cold observable was converted into a hot observable.
        test <@ observer1.ObservedValues.Length = 2 @>
        test <@ observer2.ObservedValues.Length = 2 @>
        test <@ observer1.ObservedValues = observer2.ObservedValues @>
    Prop.forAll DerivedCounterFactory.Arb test
    
[<Property>]
let ``starts emitting increments immediately`` (increment1: obj) (increment2: obj) =
    let test (DerivedCounterFactory deriveCounter) =
        // Create an observable that emits one value immediately
        // and a second value when a subject is completed.
        use subject = new Subject<obj>()
        let observable =
            Observable.Concat(
                Observable.Return(increment1),
                subject,
                Observable.Return(increment2))
        let counter = deriveCounter observable
        let observer = TestObserver()
        use subscription = counter.Subscribe(observer)
        // The first value should have been emitted before the
        // observer was subscribed if the derived observable
        // started emitting values immediately.
        test <@ observer.ObservedValues = [] @>
        // Complete the subject to emit the second value.
        subject.OnCompleted()
        // The only observed value should be the second value.
        test <@ observer.ObservedValues = [ increment2 ] @>
    Prop.forAll DerivedCounterFactory.Arb test