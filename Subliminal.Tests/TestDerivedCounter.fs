namespace Subliminal.Tests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects

type DerivedCounterFactory<'TIncrement> =
    DerivedCounterFactory of deriveCounter:(IObservable<'TIncrement> -> ICounter<'TIncrement>) with
    static member Arb =
        [ fun observable -> DerivedCounter<'TIncrement>.FromObservable(observable) :> ICounter<'TIncrement>
          fun observable -> observable.AsCounter<'TIncrement>() ]
        |> Gen.elements
        |> Gen.map DerivedCounterFactory
        |> Arb.fromGen

module ``Test DerivedCounter<TIncrement>`` =
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
        
    [<Property>]
    let ``rate returns original values as deltas`` value1 value2 value3 =
        let test (DerivedCounterFactory deriveCounter) =
            use subject = new Subject<obj>()
            let counter = deriveCounter subject
            let observable = counter.Rate()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 3 @>
            test <@ observer.ObservedValues.[0].Delta = value1 @>
            test <@ observer.ObservedValues.[1].Delta = value2 @>
            test <@ observer.ObservedValues.[2].Delta = value3 @>
        Prop.forAll DerivedCounterFactory.Arb test

    [<Property>]
    let ``rate returns intervals between values`` (value1: obj) (value2: obj) (value3: obj) =
        let test (DerivedCounterFactory deriveCounter) =
            use subject = new Subject<obj>()
            let counter = deriveCounter subject
            let observable = counter.Rate()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 3 @>
            test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
            test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
            test <@ observer.ObservedValues.[2].Interval > TimeSpan.Zero @>
        Prop.forAll DerivedCounterFactory.Arb test
        
    [<Property>]
    let ``rate returns increment selector result as delta`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedCounterFactory deriveCounter) =
            use subject = new Subject<Wrapper<obj>>()
            let counter = deriveCounter subject
            let observable = counter.Rate(fun increment -> increment.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 3 @>
            test <@ observer.ObservedValues.[0].Delta = wrapper1.Value @>
            test <@ observer.ObservedValues.[1].Delta = wrapper2.Value @>
            test <@ observer.ObservedValues.[2].Delta = wrapper3.Value @>
        Prop.forAll DerivedCounterFactory.Arb test
