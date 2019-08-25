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
    let ``delta combines values pairwise`` value1 value2 value3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<obj>()
            let gauge = deriveGauge subject
            let observable = gauge.Delta()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
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
    let ``delta combines value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<Wrapper<obj>>()
            let gauge = deriveGauge subject
            let observable = gauge.Delta(fun wrapper -> wrapper.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].PreviousValue = wrapper1.Value @>
            test <@ observer.ObservedValues.[0].CurrentValue = wrapper2.Value @>
            test <@ observer.ObservedValues.[1].PreviousValue = wrapper2.Value @>
            test <@ observer.ObservedValues.[1].CurrentValue = wrapper3.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test

    [<Property>]
    let ``delta returns delta selector results from pairwise values`` value1 value2 value3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<obj>()
            let gauge = deriveGauge subject
            let observable = gauge.Delta((fun value -> value), (fun delta -> { Value = delta }))
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Value.PreviousValue = value1 @>
            test <@ observer.ObservedValues.[0].Value.CurrentValue = value2 @>
            test <@ observer.ObservedValues.[1].Value.PreviousValue = value2 @>
            test <@ observer.ObservedValues.[1].Value.CurrentValue = value3 @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``delta returns delta selector results from pairwise value selector results`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<Wrapper<obj>>()
            let gauge = deriveGauge subject
            let observable = gauge.Delta((fun wrapper -> wrapper.Value), (fun delta -> { Value = delta }))
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Value.PreviousValue = wrapper1.Value @>
            test <@ observer.ObservedValues.[0].Value.CurrentValue = wrapper2.Value @>
            test <@ observer.ObservedValues.[1].Value.PreviousValue = wrapper2.Value @>
            test <@ observer.ObservedValues.[1].Value.CurrentValue = wrapper3.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``delta subtracts int value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<Wrapper<int>>()
            let gauge = deriveGauge subject
            let observable = gauge.Delta(fun wrapper -> wrapper.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0] = wrapper2.Value - wrapper1.Value @>
            test <@ observer.ObservedValues.[1] = wrapper3.Value - wrapper2.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``delta subtracts long value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<Wrapper<int64>>()
            let gauge = deriveGauge subject
            let observable = gauge.Delta(fun wrapper -> wrapper.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0] = wrapper2.Value - wrapper1.Value @>
            test <@ observer.ObservedValues.[1] = wrapper3.Value - wrapper2.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``delta subtracts time span value selector results pairwise`` value1 value2 value3 =
        let test (DerivedGaugeFactory deriveGauge) =
            let wrapper1 = { Value = TimeSpan.FromTicks(value1) }
            let wrapper2 = { Value = TimeSpan.FromTicks(value2) }
            let wrapper3 = { Value = TimeSpan.FromTicks(value3) }
            use subject = new Subject<Wrapper<TimeSpan>>()
            let gauge = deriveGauge subject
            let observable = gauge.Delta(fun wrapper -> wrapper.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0] = wrapper2.Value - wrapper1.Value @>
            test <@ observer.ObservedValues.[1] = wrapper3.Value - wrapper2.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test

    [<Property>]
    let ``rate of change returns intervals between pairwise values`` (value1: obj) (value2: obj) (value3: obj) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<obj>()
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
    let ``rate of change combines values pairwise`` value1 value2 value3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<obj>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange()
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta.PreviousValue = value1 @>
            test <@ observer.ObservedValues.[0].Delta.CurrentValue = value2 @>
            test <@ observer.ObservedValues.[1].Delta.PreviousValue = value2 @>
            test <@ observer.ObservedValues.[1].Delta.CurrentValue = value3 @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``rate of change combines value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<Wrapper<obj>>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange(fun wrapper -> wrapper.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta.PreviousValue = wrapper1.Value @>
            test <@ observer.ObservedValues.[0].Delta.CurrentValue = wrapper2.Value @>
            test <@ observer.ObservedValues.[1].Delta.PreviousValue = wrapper2.Value @>
            test <@ observer.ObservedValues.[1].Delta.CurrentValue = wrapper3.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test

    [<Property>]
    let ``rate of change returns delta selector results from pairwise values`` value1 value2 value3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<obj>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange((fun value -> value), (fun delta -> { Value = delta }))
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(value1)
            subject.OnNext(value2)
            subject.OnNext(value3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta.Value.PreviousValue = value1 @>
            test <@ observer.ObservedValues.[0].Delta.Value.CurrentValue = value2 @>
            test <@ observer.ObservedValues.[1].Delta.Value.PreviousValue = value2 @>
            test <@ observer.ObservedValues.[1].Delta.Value.CurrentValue = value3 @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``rate of change returns delta selector results from pairwise value selector results`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<Wrapper<obj>>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange((fun wrapper -> wrapper.Value), (fun delta -> { Value = delta }))
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta.Value.PreviousValue = wrapper1.Value @>
            test <@ observer.ObservedValues.[0].Delta.Value.CurrentValue = wrapper2.Value @>
            test <@ observer.ObservedValues.[1].Delta.Value.PreviousValue = wrapper2.Value @>
            test <@ observer.ObservedValues.[1].Delta.Value.CurrentValue = wrapper3.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``rate of change subtracts int value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<Wrapper<int>>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange(fun wrapper -> wrapper.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta = wrapper2.Value - wrapper1.Value @>
            test <@ observer.ObservedValues.[1].Delta = wrapper3.Value - wrapper2.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``rate of change subtracts long value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<Wrapper<int64>>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange(fun wrapper -> wrapper.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta = wrapper2.Value - wrapper1.Value @>
            test <@ observer.ObservedValues.[1].Delta = wrapper3.Value - wrapper2.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
    [<Property>]
    let ``rate of change subtracts time span value selector results pairwise`` value1 value2 value3 =
        let test (DerivedGaugeFactory deriveGauge) =
            let wrapper1 = { Value = TimeSpan.FromTicks(value1) }
            let wrapper2 = { Value = TimeSpan.FromTicks(value2) }
            let wrapper3 = { Value = TimeSpan.FromTicks(value3) }
            use subject = new Subject<Wrapper<TimeSpan>>()
            let gauge = deriveGauge subject
            let observable = gauge.RateOfChange(fun wrapper -> wrapper.Value)
            let observer = TestObserver()
            use subscription = observable.Subscribe(observer)
            subject.OnNext(wrapper1)
            subject.OnNext(wrapper2)
            subject.OnNext(wrapper3)
            test <@ observer.ObservedValues.Length = 2 @>
            test <@ observer.ObservedValues.[0].Delta = wrapper2.Value - wrapper1.Value @>
            test <@ observer.ObservedValues.[1].Delta = wrapper3.Value - wrapper2.Value @>
        Prop.forAll DerivedGaugeFactory.Arb test
        
module ``Test DerivedGauge<int>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<int>()
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
    let ``rate of change subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<int>()
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
        
module ``Test DerivedGauge<long>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<int64>()
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
    let ``rate of change subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let test (DerivedGaugeFactory deriveGauge) =
            use subject = new Subject<int64>()
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
        
module ``Test DerivedGauge<TimeSpan>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let test (DerivedGaugeFactory deriveGauge) =
            let value1 = TimeSpan.FromTicks(value1)
            let value2 = TimeSpan.FromTicks(value2)
            let value3 = TimeSpan.FromTicks(value3)
            use subject = new Subject<TimeSpan>()
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
    let ``rate of change subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let test (DerivedGaugeFactory deriveGauge) =
            let value1 = TimeSpan.FromTicks(value1)
            let value2 = TimeSpan.FromTicks(value2)
            let value3 = TimeSpan.FromTicks(value3)
            use subject = new Subject<TimeSpan>()
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
