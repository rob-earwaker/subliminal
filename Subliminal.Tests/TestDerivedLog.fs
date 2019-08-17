namespace Subliminal.Tests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects

module ``Test DerivedLog<TEntry>`` =
    type DerivedLogFactory =
        DerivedLogFactory of deriveLog:(IObservable<obj> -> ILog<obj>) with
        static member Arb =
            [ fun observable -> DerivedLog<obj>.FromObservable(observable) :> ILog<obj>
              fun observable -> observable.AsLog<obj>() ]
            |> Gen.elements
            |> Gen.map DerivedLogFactory
            |> Arb.fromGen

    [<Property>]
    let ``emits entries`` (entry1: obj) (entry2: obj) =
        let test (DerivedLogFactory deriveLog) =
            use subject = new Subject<obj>()
            let log = deriveLog subject
            let observer = TestObserver()
            use subscription = log.Subscribe(observer)
            subject.OnNext(entry1)
            subject.OnNext(entry2)
            test <@ observer.ObservedValues = [ entry1; entry2 ] @>
        Prop.forAll DerivedLogFactory.Arb test
    
    [<Property>]
    let ``observers receive the same entries`` () =
        let test (DerivedLogFactory deriveLog) =
            // Create an observable that emits two values after a
            // subject is completed. This is a cold observable and
            // will emit different values to each observer.
            use subject = new Subject<obj>()
            let observable =
                Observable.Concat(
                    subject,
                    Observable.Return(Unit.Default).Select(fun _ -> obj()),
                    Observable.Return(Unit.Default).Select(fun _ -> obj()))
            let log = deriveLog observable
            let observer1 = TestObserver()
            let observer2 = TestObserver()
            use subscription1 = log.Subscribe(observer1)
            use subscription2 = log.Subscribe(observer2)
            // Complete the subject to emit the values.
            subject.OnCompleted()
            // Both observers should have received the same values if the
            // cold observable was converted into a hot observable.
            test <@ observer1.ObservedValues.Length = 2 @>
            test <@ observer2.ObservedValues.Length = 2 @>
            test <@ observer1.ObservedValues = observer2.ObservedValues @>
        Prop.forAll DerivedLogFactory.Arb test
    
    [<Property>]
    let ``starts emitting entries immediately`` (entry1: obj) (entry2: obj) =
        let test (DerivedLogFactory deriveLog) =
            // Create an observable that emits one value immediately
            // and a second value when a subject is completed.
            use subject = new Subject<obj>()
            let observable =
                Observable.Concat(
                    Observable.Return(entry1),
                    subject,
                    Observable.Return(entry2))
            let log = deriveLog observable
            let observer = TestObserver()
            use subscription = log.Subscribe(observer)
            // The first value should have been emitted before the
            // observer was subscribed if the derived observable
            // started emitting values immediately.
            test <@ observer.ObservedValues = [] @>
            // Complete the subject to emit the second value.
            subject.OnCompleted()
            // The only observed value should be the second value.
            test <@ observer.ObservedValues = [ entry2 ] @>
        Prop.forAll DerivedLogFactory.Arb test
