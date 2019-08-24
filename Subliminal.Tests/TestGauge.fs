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
    let ``delta combines values pairwise`` (value1: obj) (value2: obj) (value3: obj) =
        let gauge = Gauge<obj>()
        let observer = TestObserver()
        use subscription = gauge.Delta().Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].PreviousValue = value1 @>
        test <@ observer.ObservedValues.[0].CurrentValue = value2 @>
        test <@ observer.ObservedValues.[1].PreviousValue = value2 @>
        test <@ observer.ObservedValues.[1].CurrentValue = value3 @>

    [<Property>]
    let ``delta uses custom selector result`` (value1: obj) (value2: obj) (value3: obj) (result: obj) =
        let gauge = Gauge<obj>()
        let observer = TestObserver()
        use subscription = gauge.Delta(fun delta -> result).Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues = [ result; result ] @>
        
    [<Property>]
    let ``rate of change returns intervals between pairwise values`` (value1: obj) (value2: obj) (value3: obj) =
        let gauge = Gauge<obj>()
        let observer = TestObserver()
        use subscription = gauge.RateOfChange().Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Interval > TimeSpan.Zero @>
        test <@ observer.ObservedValues.[1].Interval > TimeSpan.Zero @>
        
    [<Property>]
    let ``rate of change combines values pairwise`` (value1: obj) (value2: obj) (value3: obj) =
        let gauge = Gauge<obj>()
        let observer = TestObserver()
        use subscription = gauge.RateOfChange().Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta.PreviousValue = value1 @>
        test <@ observer.ObservedValues.[0].Delta.CurrentValue = value2 @>
        test <@ observer.ObservedValues.[1].Delta.PreviousValue = value2 @>
        test <@ observer.ObservedValues.[1].Delta.CurrentValue = value3 @>
        
    [<Property>]
    let ``rate of change uses custom delta selector result`` (value1: obj) (value2: obj) (value3: obj) (result: obj) =
        let gauge = Gauge<obj>()
        let observer = TestObserver()
        use subscription = gauge.RateOfChange(fun delta -> result).Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = result @>
        test <@ observer.ObservedValues.[1].Delta = result @>
        
module ``Test Gauge<int>`` =
    [<Property>]
    let ``delta subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let gauge = Gauge<int>()
        let observer = TestObserver()
        use subscription = gauge.Delta().Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
    [<Property>]
    let ``rate of change subtracts pairwise values`` (value1: int) (value2: int) (value3: int) =
        let gauge = Gauge<int>()
        let observer = TestObserver()
        use subscription = gauge.RateOfChange().Subscribe(observer)
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
        let observer = TestObserver()
        use subscription = gauge.Delta().Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0] = value2 - value1 @>
        test <@ observer.ObservedValues.[1] = value3 - value2 @>
        
    [<Property>]
    let ``rate of change subtracts pairwise values`` (value1: int64) (value2: int64) (value3: int64) =
        let gauge = Gauge<int64>()
        let observer = TestObserver()
        use subscription = gauge.RateOfChange().Subscribe(observer)
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
        let observer = TestObserver()
        use subscription = gauge.Delta().Subscribe(observer)
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
        let observer = TestObserver()
        use subscription = gauge.RateOfChange().Subscribe(observer)
        gauge.LogValue(value1)
        gauge.LogValue(value2)
        gauge.LogValue(value3)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].Delta = value2 - value1 @>
        test <@ observer.ObservedValues.[1].Delta = value3 - value2 @>
