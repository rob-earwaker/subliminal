namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System

module ``Test Gauge<TValue>`` =
    [<Property>]
    let ``emits values when logged`` (value1: obj) (value2: obj) =
        let gauge = Gauge<obj>()
        let observer = TestObserver()
        use subscription = gauge.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        test <@ observer.ObservedValues = [ value1; value2 ] @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``observers receive the same values`` (value1: obj) (value2: obj) =
        let gauge = Gauge<obj>()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = gauge.Subscribe(observer1)
        use subscription2 = gauge.Subscribe(observer2)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        test <@ observer1.ObservedValues = [ value1; value2 ] @>
        test <@ observer2.ObservedValues = [ value1; value2 ] @>
        
    [<Property>]
    let ``delta combines values pairwise`` value1 value2 value3 =
        let gauge = Gauge<obj>()
        let observable = gauge.Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].PreviousValue = value1 @>
        test <@ observer.ObservedValues.[0].CurrentValue = value2 @>
        test <@ observer.ObservedValues.[1].PreviousValue = value2 @>
        test <@ observer.ObservedValues.[1].CurrentValue = value3 @>
        
    [<Property>]
    let ``delta combines value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let gauge = Gauge<Wrapper<obj>>()
        let observable = gauge.Delta(fun wrapper -> wrapper.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].PreviousValue = wrapper1.Value @>
        test <@ observer.ObservedValues.[0].CurrentValue = wrapper2.Value @>
        test <@ observer.ObservedValues.[1].PreviousValue = wrapper2.Value @>
        test <@ observer.ObservedValues.[1].CurrentValue = wrapper3.Value @>

    [<Property>]
    let ``delta returns delta selector results from pairwise values`` value1 value2 value3 =
        let gauge = Gauge<obj>()
        let observable = gauge.Delta((fun value -> value), (fun delta -> { Value = delta }))
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Value.PreviousValue = value1 @>
        test <@ observer.ObservedValues.[0].Value.CurrentValue = value2 @>
        test <@ observer.ObservedValues.[1].Value.PreviousValue = value2 @>
        test <@ observer.ObservedValues.[1].Value.CurrentValue = value3 @>
        
    [<Property>]
    let ``delta returns delta selector results from pairwise value selector results`` wrapper1 wrapper2 wrapper3 =
        let gauge = Gauge<Wrapper<obj>>()
        let observable = gauge.Delta((fun wrapper -> wrapper.Value), (fun delta -> { Value = delta }))
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Value.PreviousValue = wrapper1.Value @>
        test <@ observer.ObservedValues.[0].Value.CurrentValue = wrapper2.Value @>
        test <@ observer.ObservedValues.[1].Value.PreviousValue = wrapper2.Value @>
        test <@ observer.ObservedValues.[1].Value.CurrentValue = wrapper3.Value @>
        
    [<Property>]
    let ``delta subtracts int value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let gauge = Gauge<Wrapper<int>>()
        let observable = gauge.Delta(fun wrapper -> wrapper.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = wrapper2.Value - wrapper1.Value @>
        test <@ observer.ObservedValues.[1] = wrapper3.Value - wrapper2.Value @>
        
    [<Property>]
    let ``delta subtracts long value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let gauge = Gauge<Wrapper<int64>>()
        let observable = gauge.Delta(fun wrapper -> wrapper.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = wrapper2.Value - wrapper1.Value @>
        test <@ observer.ObservedValues.[1] = wrapper3.Value - wrapper2.Value @>
        
    [<Property>]
    let ``delta subtracts time span value selector results pairwise`` value1 value2 value3 =
        let wrapper1 = { Value = TimeSpan.FromTicks(value1) }
        let wrapper2 = { Value = TimeSpan.FromTicks(value2) }
        let wrapper3 = { Value = TimeSpan.FromTicks(value3) }
        let gauge = Gauge<Wrapper<TimeSpan>>()
        let observable = gauge.Delta(fun wrapper -> wrapper.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = wrapper2.Value - wrapper1.Value @>
        test <@ observer.ObservedValues.[1] = wrapper3.Value - wrapper2.Value @>
        
    [<Property>]
    let ``rate of change returns intervals between pairwise values`` (value1: obj) (value2: obj) (value3: obj) =
        let gauge = Gauge<obj>()
        let observable = gauge.RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
        
    [<Property>]
    let ``rate of change combines values pairwise`` value1 value2 value3 =
        let gauge = Gauge<obj>()
        let observable = gauge.RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta.PreviousValue = value1 @>
        test <@ observer.ObservedValues.[0].Delta.CurrentValue = value2 @>
        test <@ observer.ObservedValues.[1].Delta.PreviousValue = value2 @>
        test <@ observer.ObservedValues.[1].Delta.CurrentValue = value3 @>
        
    [<Property>]
    let ``rate of change combines value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let gauge = Gauge<Wrapper<obj>>()
        let observable = gauge.RateOfChange(fun wrapper -> wrapper.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta.PreviousValue = wrapper1.Value @>
        test <@ observer.ObservedValues.[0].Delta.CurrentValue = wrapper2.Value @>
        test <@ observer.ObservedValues.[1].Delta.PreviousValue = wrapper2.Value @>
        test <@ observer.ObservedValues.[1].Delta.CurrentValue = wrapper3.Value @>

    [<Property>]
    let ``rate of change returns delta selector results from pairwise values`` value1 value2 value3 =
        let gauge = Gauge<obj>()
        let observable = gauge.RateOfChange((fun value -> value), (fun delta -> { Value = delta }))
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta.Value.PreviousValue = value1 @>
        test <@ observer.ObservedValues.[0].Delta.Value.CurrentValue = value2 @>
        test <@ observer.ObservedValues.[1].Delta.Value.PreviousValue = value2 @>
        test <@ observer.ObservedValues.[1].Delta.Value.CurrentValue = value3 @>
        
    [<Property>]
    let ``rate of change returns delta selector results from pairwise value selector results`` wrapper1 wrapper2 wrapper3 =
        let gauge = Gauge<Wrapper<obj>>()
        let observable = gauge.RateOfChange((fun wrapper -> wrapper.Value), (fun delta -> { Value = delta }))
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta.Value.PreviousValue = wrapper1.Value @>
        test <@ observer.ObservedValues.[0].Delta.Value.CurrentValue = wrapper2.Value @>
        test <@ observer.ObservedValues.[1].Delta.Value.PreviousValue = wrapper2.Value @>
        test <@ observer.ObservedValues.[1].Delta.Value.CurrentValue = wrapper3.Value @>
        
    [<Property>]
    let ``rate of change subtracts int value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let gauge = Gauge<Wrapper<int>>()
        let observable = gauge.RateOfChange(fun wrapper -> wrapper.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = wrapper2.Value - wrapper1.Value @>
        test <@ observer.ObservedValues.[1].Delta = wrapper3.Value - wrapper2.Value @>
        
    [<Property>]
    let ``rate of change subtracts long value selector results pairwise`` wrapper1 wrapper2 wrapper3 =
        let gauge = Gauge<Wrapper<int64>>()
        let observable = gauge.RateOfChange(fun wrapper -> wrapper.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = wrapper2.Value - wrapper1.Value @>
        test <@ observer.ObservedValues.[1].Delta = wrapper3.Value - wrapper2.Value @>
        
    [<Property>]
    let ``rate of change subtracts time span value selector results pairwise`` value1 value2 value3 =
        let wrapper1 = { Value = TimeSpan.FromTicks(value1) }
        let wrapper2 = { Value = TimeSpan.FromTicks(value2) }
        let wrapper3 = { Value = TimeSpan.FromTicks(value3) }
        let gauge = Gauge<Wrapper<TimeSpan>>()
        let observable = gauge.RateOfChange(fun wrapper -> wrapper.Value)
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(wrapper1)
        gauge.LogValue(wrapper2)
        gauge.LogValue(wrapper3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = wrapper2.Value - wrapper1.Value @>
        test <@ observer.ObservedValues.[1].Delta = wrapper3.Value - wrapper2.Value @>
        
module ``Test Gauge<int>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let gauge = Gauge<int>()
        let observable = gauge.Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
    [<Property>]
    let ``rate of change subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let gauge = Gauge<int>()
        let observable = gauge.RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
        test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
        
module ``Test Gauge<long>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let gauge = Gauge<int64>()
        let observable = gauge.Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
    [<Property>]
    let ``rate of change subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let gauge = Gauge<int64>()
        let observable = gauge.RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
        test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
        
module ``Test Gauge<TimeSpan>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let value1 = TimeSpan.FromTicks(value1)
        let value2 = TimeSpan.FromTicks(value2)
        let value3 = TimeSpan.FromTicks(value3)
        let gauge = Gauge<TimeSpan>()
        let observable = gauge.Delta()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
    [<Property>]
    let ``rate of change subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let value1 = TimeSpan.FromTicks(value1)
        let value2 = TimeSpan.FromTicks(value2)
        let value3 = TimeSpan.FromTicks(value3)
        let gauge = Gauge<TimeSpan>()
        let observable = gauge.RateOfChange()
        let observer = TestObserver()
        use subscription = observable.Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
        test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
