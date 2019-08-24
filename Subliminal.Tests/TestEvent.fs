namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System.Reactive

module ``Test Event<TEvent>`` =
    [<Property>]
    let ``emits value when raised`` (value: obj) =
        let event = Event<obj>()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise(value)
        test <@ observer.ObservedValues = [ value ] @>
        
    [<Property>]
    let ``only emits first value`` (value1: obj) (value2: obj) =
        let event = Event<obj>()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise(value1)
        event.Raise(value2)
        test <@ observer.ObservedValues = [ value1 ] @>
        
    [<Property>]
    let ``does not complete before being raised`` () =
        let event = Event<obj>()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``completes after being raised`` (value: obj) =
        let event = Event<obj>()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise(value)
        test <@ observer.ObservableCompleted @>
        
    [<Property>]
    let ``observers receive the same value`` (value: obj) =
        let event = Event<obj>()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = event.Subscribe(observer1)
        use subscription2 = event.Subscribe(observer2)
        event.Raise(value)
        test <@ observer1.ObservedValues = [ value ] @>
        test <@ observer2.ObservedValues = [ value ] @>
        
    [<Property>]
    let ``emits value to observers that subscribe after event raised`` (value: obj) =
        let event = Event<obj>()
        event.Raise(value)
        let observer1 = TestObserver()
        use subscription1 = event.Subscribe(observer1)
        test <@ observer1.ObservedValues = [ value ] @>
        test <@ observer1.ObservableCompleted @>
        let observer2 = TestObserver()
        use subscription2 = event.Subscribe(observer2)
        test <@ observer2.ObservedValues = [ value ] @>
        test <@ observer2.ObservableCompleted @>
        
module ``Test Event`` =
    [<Property>]
    let ``emits value when raised`` () =
        let event = Event()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise()
        test <@ observer.ObservedValues = [ Unit.Default ] @>
        
    [<Property>]
    let ``only emits one value`` () =
        let event = Event()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise()
        event.Raise()
        test <@ observer.ObservedValues = [ Unit.Default ] @>
        
    [<Property>]
    let ``does not complete before being raised`` () =
        let event = Event()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``completes after being raised`` () =
        let event = Event()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise()
        test <@ observer.ObservableCompleted @>
        
    [<Property>]
    let ``all observers receive the value`` () =
        let event = Event()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = event.Subscribe(observer1)
        use subscription2 = event.Subscribe(observer2)
        event.Raise()
        test <@ observer1.ObservedValues = [ Unit.Default ] @>
        test <@ observer2.ObservedValues = [ Unit.Default ] @>
        
    [<Property>]
    let ``emits value to observers that subscribe after event raised`` () =
        let event = Event()
        event.Raise()
        let observer1 = TestObserver()
        use subscription1 = event.Subscribe(observer1)
        test <@ observer1.ObservedValues = [ Unit.Default ] @>
        test <@ observer1.ObservableCompleted @>
        let observer2 = TestObserver()
        use subscription2 = event.Subscribe(observer2)
        test <@ observer2.ObservedValues = [ Unit.Default ] @>
        test <@ observer2.ObservableCompleted @>
        