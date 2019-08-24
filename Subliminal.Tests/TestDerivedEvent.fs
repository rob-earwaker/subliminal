namespace Subliminal.Tests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects

type DerivedEventFactory<'TEvent> =
    DerivedEventFactory of deriveEvent:(IObservable<'TEvent> -> IEvent<'TEvent>) with
    static member Arb =
        [ fun observable -> DerivedEvent<'TEvent>.FromObservable(observable) :> IEvent<'TEvent>
          fun observable -> observable.AsEvent<'TEvent>() ]
        |> Gen.elements
        |> Gen.map DerivedEventFactory
        |> Arb.fromGen

module ``Test DerivedEvent<TEvent>`` =
    [<Property>]
    let ``only emits first value`` (value1: obj) (value2: obj) =
        let test (DerivedEventFactory deriveEvent) =
            use subject = new Subject<obj>()
            let event = deriveEvent subject
            let observer = TestObserver()
            use subscription = event.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            test <@ observer.ObservedValues = [ value1 ] @>
            test <@ observer.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
    
    [<Property>]
    let ``does not complete before value is emitted`` () =
        let test (DerivedEventFactory deriveEvent) =
            use subject = new Subject<obj>()
            let event = deriveEvent subject
            let observer = TestObserver()
            use subscription = event.Subscribe(observer)
            test <@ not observer.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
        
    [<Property>]
    let ``observers receive the same value`` () =
        let test (DerivedEventFactory deriveEvent) =
            // Create an observable that emits a value after a
            // subject is completed. This is a cold observable and
            // will emit a different value to each observer.
            use subject = new Subject<obj>()
            let observable =
                Observable.Concat(
                    subject,
                    Observable.Return(Unit.Default).Select(fun _ -> obj()))
            let event = deriveEvent observable
            let observer1 = TestObserver()
            let observer2 = TestObserver()
            use subscription1 = event.Subscribe(observer1)
            use subscription2 = event.Subscribe(observer2)
            // Complete the subject to emit the value.
            subject.OnCompleted()
            // Both observers should have received the same value if the
            // cold observable was converted into a hot observable.
            test <@ observer1.ObservedValues.Length = 1 @>
            test <@ observer2.ObservedValues.Length = 1 @>
            test <@ observer1.ObservedValues = observer2.ObservedValues @>
            test <@ observer1.ObservableCompleted @>
            test <@ observer2.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
        
    [<Property>]
    let ``emits value immediately for non-empty observable`` (value: obj) =
        let test (DerivedEventFactory deriveEvent) =
            let observable = Observable.Return(value)
            let event = deriveEvent observable
            let observer = TestObserver()
            use subscription = event.Subscribe(observer)
            test <@ observer.ObservedValues = [ value ] @>
            test <@ observer.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
        
    [<Property>]
    let ``emits value to observers that subscribe after completion`` (value: obj) =
        let test (DerivedEventFactory deriveEvent) =
            use subject = new Subject<obj>()
            let event = deriveEvent subject
            subject.OnNext(value)
            let observer1 = TestObserver()
            use subscription1 = event.Subscribe(observer1)
            test <@ observer1.ObservedValues = [ value ] @>
            test <@ observer1.ObservableCompleted @>
            let observer2 = TestObserver()
            use subscription2 = event.Subscribe(observer2)
            test <@ observer2.ObservedValues = [ value ] @>
            test <@ observer2.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
        
type DerivedEventFactory =
    DerivedEventFactory of deriveEvent:(IObservable<Unit> -> IEvent) with
    static member Arb =
        [ fun observable -> DerivedEvent.FromObservable(observable) :> IEvent
          fun observable -> observable.AsEvent() ]
        |> Gen.elements
        |> Gen.map DerivedEventFactory
        |> Arb.fromGen
            
module ``Test DerivedEvent`` =
    [<Property>]
    let ``only emits single value`` () =
        let test (DerivedEventFactory deriveEvent) =
            use subject = new Subject<Unit>()
            let event = deriveEvent subject
            let observer = TestObserver()
            use subscription = event.Subscribe(observer)
            subject.OnNext(Unit.Default)
            subject.OnNext(Unit.Default)
            test <@ observer.ObservedValues = [ Unit.Default ] @>
            test <@ observer.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
            
    [<Property>]
    let ``does not complete before value is emitted`` () =
        let test (DerivedEventFactory deriveEvent) =
            use subject = new Subject<Unit>()
            let event = deriveEvent subject
            let observer = TestObserver()
            use subscription = event.Subscribe(observer)
            test <@ not observer.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
                
    [<Property>]
    let ``emits value immediately for non-empty observable`` () =
        let test (DerivedEventFactory deriveEvent) =
            let observable = Observable.Return(Unit.Default)
            let event = deriveEvent observable
            let observer = TestObserver()
            use subscription = event.Subscribe(observer)
            test <@ observer.ObservedValues = [ Unit.Default ] @>
            test <@ observer.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
                
    [<Property>]
    let ``emits value to observers that subscribe after completion`` () =
        let test (DerivedEventFactory deriveEvent) =
            use subject = new Subject<Unit>()
            let event = deriveEvent subject
            subject.OnNext(Unit.Default)
            let observer1 = TestObserver()
            use subscription1 = event.Subscribe(observer1)
            test <@ observer1.ObservedValues = [ Unit.Default ] @>
            test <@ observer1.ObservableCompleted @>
            let observer2 = TestObserver()
            use subscription2 = event.Subscribe(observer2)
            test <@ observer2.ObservedValues = [ Unit.Default ] @>
            test <@ observer2.ObservableCompleted @>
        Prop.forAll DerivedEventFactory.Arb test
