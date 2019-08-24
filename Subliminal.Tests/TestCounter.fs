namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

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
