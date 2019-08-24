namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System

module ``Test Counter<TIncrement>`` =
    [<Property>]
    let ``emits increments when incremented`` (increment1: obj) (increment2: obj) =
        let counter = Counter<obj>()
        let observer = TestObserver()
        use subscription = counter.Subscribe(observer)
        counter.IncrementBy(increment1)
        counter.IncrementBy(increment2)
        test <@ observer.ObservedValues = [ increment1; increment2 ] @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``observers receive the same increments`` (increment1: obj) (increment2: obj) =
        let counter = Counter<obj>()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = counter.Subscribe(observer1)
        use subscription2 = counter.Subscribe(observer2)
        counter.IncrementBy(increment1)
        counter.IncrementBy(increment2)
        test <@ observer1.ObservedValues = [ increment1; increment2 ] @>
        test <@ observer2.ObservedValues = [ increment1; increment2 ] @>
        
    [<Property>]
    let ``rate returns original values as deltas`` value1 value2 value3 =
        let counter = Counter<obj>()
        let observable = counter.Rate()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        counter.IncrementBy(value1)
        counter.IncrementBy(value2)
        counter.IncrementBy(value3)
        test <@ observer.ObservedValues.Length = 3 @>
        test <@ observer.ObservedValues.[0].Delta = value1 @>
        test <@ observer.ObservedValues.[1].Delta = value2 @>
        test <@ observer.ObservedValues.[2].Delta = value3 @>

    [<Property>]
    let ``rate returns intervals between values`` (value1: obj) (value2: obj) (value3: obj) =
        let counter = Counter<obj>()
        let observable = counter.Rate()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        counter.IncrementBy(value1)
        counter.IncrementBy(value2)
        counter.IncrementBy(value3)
        test <@ observer.ObservedValues.Length = 3 @>
        test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[2].Interval > TimeSpan.Zero @>
        
    [<Property>]
    let ``rate returns increment selector result as delta`` wrapper1 wrapper2 wrapper3 =
        let counter = Counter<Wrapper<obj>>()
        let observable = counter.Rate(fun increment -> increment.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        counter.IncrementBy(wrapper1)
        counter.IncrementBy(wrapper2)
        counter.IncrementBy(wrapper3)
        test <@ observer.ObservedValues.Length = 3 @>
        test <@ observer.ObservedValues.[0].Delta = wrapper1.Value @>
        test <@ observer.ObservedValues.[1].Delta = wrapper2.Value @>
        test <@ observer.ObservedValues.[2].Delta = wrapper3.Value @>

module ``Test Counter<int>`` =
    [<Property>]
    let ``emits increment of one when incremented`` () =
        let counter = Counter<int>()
        let observer = TestObserver()
        use subscription = counter.Subscribe(observer)
        counter.Increment()
        test <@ observer.ObservedValues = [ 1 ] @>
        test <@ not observer.ObservableCompleted @>
        
module ``Test Counter<long>`` =
    [<Property>]
    let ``emits increment of one when incremented`` () =
        let counter = Counter<int64>()
        let observer = TestObserver()
        use subscription = counter.Subscribe(observer)
        counter.Increment()
        test <@ observer.ObservedValues = [ 1L ] @>
        test <@ not observer.ObservableCompleted @>
