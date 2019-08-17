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
    let ``completes after being raised`` (value: obj) =
        let event = Event<obj>()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise(value)
        test <@ observer.ObservedValues = [ value ] @>
        test <@ observer.ObservableCompleted @>
        
    [<Property>]
    let ``only emits first value`` (value1: obj) (value2: obj) =
        let event = Event<obj>()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise(value1)
        event.Raise(value2)
        test <@ observer.ObservedValues = [ value1 ] @>
        
module ``Test Event`` =
    [<Property>]
    let ``emits value when raised`` () =
        let event = Event()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise()
        test <@ observer.ObservedValues = [ Unit.Default ] @>
        
    [<Property>]
    let ``completes after being raised`` () =
        let event = Event()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise()
        test <@ observer.ObservedValues = [ Unit.Default ] @>
        test <@ observer.ObservableCompleted @>
        
    [<Property>]
    let ``only emits one value`` () =
        let event = Event()
        let observer = TestObserver()
        use subscription = event.Subscribe(observer)
        event.Raise()
        event.Raise()
        test <@ observer.ObservedValues = [ Unit.Default ] @>
        