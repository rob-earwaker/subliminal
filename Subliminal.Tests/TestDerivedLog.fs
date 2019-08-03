module Subliminal.Tests.TestDerivedLog

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Collections.Generic
open System.Reactive.Linq
open System.Reactive.Subjects
open Xunit

[<Fact>]
let ``observable is published`` () =
    use trigger = new Subject<obj>()
    let source =
        // Create a source observable that produces a single
        // object. Subscribing to this observable directly
        // would yield a different object per observer.
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(obj())
            Action(id))
    // Create an observable that will only produce the source
    // observable's object when the trigger is pulled.
    let observable =
        Observable
            .Zip(source, trigger)
            .Select(fun zip -> zip.[0])
    let log = DerivedLog.FromObservable(observable)
    let observer1 = TestObserver()
    let observer2 = TestObserver()
    use subscription1 = log.Subscribe(observer1)
    use subscription2 = log.Subscribe(observer2)
    // Pull the trigger to emit the object from the source.
    trigger.OnNext(obj())
    test <@ not observer1.ObservableCompleted @>
    test <@ not observer2.ObservableCompleted @>
    test <@ observer1.ObservedValues.Count = 1 @>
    test <@ observer2.ObservedValues.Count = 1 @>
    // If the source observable was published in the process of
    // turning it into a log, both observers should have received
    // the same object from the source.
    test <@ observer1.ObservedValues.[0] = observer2.ObservedValues.[0] @>
    
[<Property>]
let ``observable is connected`` sourceObj =
    use trigger = new Subject<obj>()
    let source =
        // Create a source observable that produces a single object.
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(sourceObj)
            Action(id))
    // Create an observable that returns an initial object immediately
    // and then the source object once the trigger is pulled.
    let observable =
        Observable
            .Return<IList<obj>>([| obj(); obj() |])
            .Concat(Observable.Zip(source, trigger))
            .Select(fun zip -> zip.[0])
    let log = DerivedLog.FromObservable(observable)
    let observer = TestObserver()
    use subscription = log.Subscribe(observer)
    // If the source observable was connected in the process of turning
    // it into a log, the initial object should have been emitted before
    // the observer was subscribed.
    test <@ observer.ObservedValues.Count = 0 @>
    // Pull the trigger to emit the object from the source.
    trigger.OnNext(obj())
    // The only observed object should be the source object.
    test <@ observer.ObservedValues.Count = 1 @>
    test <@ observer.ObservedValues.[0] = sourceObj @>

