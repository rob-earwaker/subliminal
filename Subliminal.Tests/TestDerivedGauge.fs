namespace Subliminal.Tests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects

type DerivedGaugeFactory<'TValue> =
    DerivedGaugeFactory of deriveGauge:(IObservable<'TValue> -> IGauge<'TValue>) with
    static member Arb =
        [ fun observable -> DerivedGauge<'TValue>.FromObservable(observable) :> IGauge<'TValue>
          fun observable -> observable.AsGauge<'TValue>() ]
        |> Gen.elements
        |> Gen.map DerivedGaugeFactory
        |> Arb.fromGen

module ``Test DerivedGauge<TValue>`` =
    [<Property>]
    let ``emits values`` (value1: obj) (value2: obj) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<obj>()
            let gauge = deriveGauge subject
            let observer = TestObserver()
            use subscription = gauge.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            test <@ observer.ObservedValues = [ value1; value2 ] @>
        Prop.forAll DerivedGaugeFactory.Arb test
    
    [<Property>]
    let ``observers receive the same values`` () =
        let test (DerivedGaugeFactory deriveGauge) =
            // Create an observable that emits two values after a
            // subject is completed. This is a cold observable and
            // will emit different values to each observer.
            use subject = new Subject<obj>()
            let observable =
                Observable.Concat(
                    subject,
                    Observable.Return(Unit.Default).Select(fun _ -> obj()),
                    Observable.Return(Unit.Default).Select(fun _ -> obj()))
            let gauge = deriveGauge observable
            let observer1 = TestObserver()
            let observer2 = TestObserver()
            use subscription1 = gauge.Subscribe(observer1)
            use subscription2 = gauge.Subscribe(observer2)
            // Complete the subject to emit the values.
            subject.OnCompleted()
            // Both observers should have received the same values if the
            // cold observable was converted into a hot observable.
            test <@ observer1.ObservedValues.Length = 2 @>
            test <@ observer2.ObservedValues.Length = 2 @>
            test <@ observer1.ObservedValues = observer2.ObservedValues @>
        Prop.forAll DerivedGaugeFactory.Arb test
    
    [<Property>]
    let ``starts emitting values immediately`` (value1: obj) (value2: obj) =
        let test (DerivedGaugeFactory deriveGauge) =
            // Create an observable that emits one value immediately
            // and a second value when a subject is completed.
            use subject = new Subject<obj>()
            let observable =
                Observable.Concat(
                    Observable.Return(value1),
                    subject,
                    Observable.Return(value2))
            let gauge = deriveGauge observable
            let observer = TestObserver()
            use subscription = gauge.Subscribe(observer)
            // The first value should have been emitted before the
            // observer was subscribed if the derived observable
            // started emitting values immediately.
            test <@ observer.ObservedValues = [] @>
            // Complete the subject to emit the second value.
            subject.OnCompleted()
            // The only observed value should be the second value.
            test <@ observer.ObservedValues = [ value2 ] @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``delta combines values pairwise`` (value1: obj) (value2: obj) (value3: obj) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<obj>()
            let gauge = deriveGauge subject
            let observer = TestObserver()
            use subscription = gauge.Delta().Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].PreviousValue = value1 @>
            test <@ observer.ObservedValues.[0].CurrentValue = value2 @>
            test <@ observer.ObservedValues.[1].PreviousValue = value2 @>
            test <@ observer.ObservedValues.[1].CurrentValue = value3 @>
        Prop.forAll DerivedGaugeFactory.Arb test

    [<Property>]
    let ``delta uses custom selector result`` (value1: obj) (value2: obj) (value3: obj) (result: obj) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<obj>()
            let gauge = deriveGauge subject
            let observer = TestObserver()
            use subscription = gauge.Delta(fun delta -> result).Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues = [ result; result ] @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
module ``Test DerivedGauge<int>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<int>()
            let gauge = deriveGauge subject
            let observer = TestObserver()
            use subscription = gauge.Delta().Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0] = value2 - value1 @>
            test <@ observer.ObservedValues.[1] = value3 - value2 @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
module ``Test DerivedGauge<long>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<int64>()
            let gauge = deriveGauge subject
            let observer = TestObserver()
            use subscription = gauge.Delta().Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0] = value2 - value1 @>
            test <@ observer.ObservedValues.[1] = value3 - value2 @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
module ``Test DerivedGauge<TimeSpan>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let test (DerivedGaugeFactory deriveGauge) =
            let value1 = TimeSpan.FromTicks(value1)
            let value2 = TimeSpan.FromTicks(value2)
            let value3 = TimeSpan.FromTicks(value3)
            use subject = new Subject<TimeSpan>()
            let gauge = deriveGauge subject
            let observer = TestObserver()
            use subscription = gauge.Delta().Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0] = value2 - value1 @>
            test <@ observer.ObservedValues.[1] = value3 - value2 @>
        Prop.forAll DerivedGaugeFactory.Arb test
