﻿module Subliminal.Tests.TestDerivedLog

open Subliminal
open Swensen.Unquote
open System
open System.Collections.Generic
open System.Reactive.Linq
open System.Reactive.Subjects
open Xunit

let ``derived log factories`` = [|
    [| fun observable -> DerivedCounter<obj>.FromObservable(observable) :> IObservable<obj> |]
    [| fun observable -> DerivedEventLog<obj>.FromObservable(observable) :> IObservable<obj> |]
    [| fun observable -> DerivedGauge<obj>.FromObservable(observable) :> IObservable<obj> |]
    [| fun (observable: IObservable<obj>) -> observable.AsCounter() :> IObservable<obj> |]
    [| fun (observable: IObservable<obj>) -> observable.AsEventLog() :> IObservable<obj> |]
    [| fun (observable: IObservable<obj>) -> observable.AsGauge() :> IObservable<obj> |] |]

[<Theory>]
[<MemberData("derived log factories")>]
let ``publishes source observable`` (createLog: IObservable<obj> -> IObservable<obj>) =
    use trigger = new Subject<obj>()
    let source =
        // Create a source observable that produces a single
        // value. Subscribing to this observable directly
        // would yield a different value per observer.
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(obj())
            Action(ignore))
    // Create an observable that will only produce the source
    // observable's value when the trigger is pulled.
    let observable =
        Observable
            .Zip(source, trigger)
            .Select(fun zip -> zip.[0])
    let log = createLog observable
    let observer1 = TestObserver()
    let observer2 = TestObserver()
    use subscription1 = log.Subscribe(observer1)
    use subscription2 = log.Subscribe(observer2)
    // Pull the trigger to emit the source value.
    trigger.OnNext(obj())
    test <@ not observer1.ObservableCompleted @>
    test <@ not observer2.ObservableCompleted @>
    // If the source observable was published, both observers
    // should have received the same value from the source.
    test <@ observer1.ObservedValues = observer2.ObservedValues @>
    
[<Theory>]
[<MemberData("derived log factories")>]
let ``connects to source observable`` (createLog: IObservable<obj> -> IObservable<obj>) =
    use trigger = new Subject<obj>()
    let value = obj()
    let source =
        // Create a source observable that produces a single value.
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(value)
            Action(ignore))
    // Create an observable that returns an initial value immediately
    // and then the source value once the trigger is pulled.
    let observable =
        Observable
            .Return<IList<obj>>([| obj(); obj() |])
            .Concat(Observable.Zip(source, trigger))
            .Select(fun zip -> zip.[0])
    let log = createLog observable
    let observer = TestObserver()
    use subscription = log.Subscribe(observer)
    // If the source observable was connected, the initial value
    // should have been emitted before the observer was subscribed.
    test <@ observer.ObservedValues = [] @>
    // Pull the trigger to emit the source value.
    trigger.OnNext(obj())
    // The only observed value should be the source value.
    test <@ observer.ObservedValues = [ value ] @>