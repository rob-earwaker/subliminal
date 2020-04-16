namespace Subliminal.Tests

open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects

type DerivedGaugeFactory =
    DerivedGaugeFactory of deriveGauge:(IObservable<float> -> IGauge) with
    static member Arb =
        [ fun observable -> DerivedGauge.FromObservable(observable) :> IGauge
          fun observable -> observable.AsGauge() ]
        |> Gen.elements
        |> Gen.map DerivedGaugeFactory
        |> Arb.fromGen

module ``Test DerivedGauge`` =
    [<Property>]
    let ``emits values`` (NormalFloat value1) (NormalFloat value2) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<float>()
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
            use subject = new Subject<float>()
            let observable =
                Observable.Concat(
                    subject,
                    Observable.Return(Unit.Default).Select(fun _ -> 1.23),
                    Observable.Return(Unit.Default).Select(fun _ -> 4.56))
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
    let ``starts emitting values immediately`` (NormalFloat value1) (NormalFloat value2) =
        let test (DerivedGaugeFactory deriveGauge) =
            // Create an observable that emits one value immediately
            // and a second value when a subject is completed.
            use subject = new Subject<float>()
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
    let ``delta subtracts values pairwise`` (NormalFloat value1) (NormalFloat value2) (NormalFloat value3) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<float>()
            let gauge = deriveGauge subject
            let observable = gauge.Delta()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0] = value2 - value1 @>
            test <@ observer.ObservedValues.[1] = value3 - value2 @>
        Prop.forAll DerivedGaugeFactory.Arb test

    [<Property>]
    let ``rate of change returns intervals between pairwise values`` (NormalFloat value1) (NormalFloat value2) (NormalFloat value3) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<float>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
            test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``rate of change subtracts values pairwise`` (NormalFloat value1) (NormalFloat value2) (NormalFloat value3) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<float>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
            test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
        Prop.forAll DerivedGaugeFactory.Arb test
