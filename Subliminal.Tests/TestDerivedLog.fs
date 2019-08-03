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
let ``publishes source observable`` () =
    use trigger = new Subject<obj>()
    let source =
        // Create a source observable that produces a single
        // entry. Subscribing to this observable directly
        // would yield a different entry per observer.
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(obj())
            Action(ignore))
    // Create an observable that will only produce the source
    // observable's entry when the trigger is pulled.
    let observable =
        Observable
            .Zip(source, trigger)
            .Select(fun zip -> zip.[0])
    let log = DerivedLog.FromObservable(observable)
    let observer1 = TestObserver()
    let observer2 = TestObserver()
    use subscription1 = log.Subscribe(observer1)
    use subscription2 = log.Subscribe(observer2)
    // Pull the trigger to emit the source entry.
    trigger.OnNext(obj())
    test <@ not observer1.ObservableCompleted @>
    test <@ not observer2.ObservableCompleted @>
    // If the source observable was published in the process of
    // turning it into a log, both observers should have received
    // the same entry from the source.
    test <@ observer1.ObservedValues = observer2.ObservedValues @>
    
[<Property>]
let ``connects to source observable`` (entry: obj) =
    use trigger = new Subject<obj>()
    let source =
        // Create a source observable that produces a single entry.
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(entry)
            Action(ignore))
    // Create an observable that returns an initial entry immediately
    // and then the source entry once the trigger is pulled.
    let observable =
        Observable
            .Return<IList<obj>>([| obj(); obj() |])
            .Concat(Observable.Zip(source, trigger))
            .Select(fun zip -> zip.[0])
    let log = DerivedLog.FromObservable(observable)
    let observer = TestObserver()
    use subscription = log.Subscribe(observer)
    // If the source observable was connected in the process of turning
    // it into a log, the initial entry should have been emitted before
    // the observer was subscribed.
    test <@ observer.ObservedValues = [] @>
    // Pull the trigger to emit the source entry.
    trigger.OnNext(obj())
    // The only observed value should be the source entry.
    test <@ observer.ObservedValues = [ entry ] @>

