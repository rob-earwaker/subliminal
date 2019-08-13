module Subliminal.Tests.TestDerivedEvent

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive.Linq
open System.Reactive.Subjects

[<Property>]
let ``emits first event from source observable`` (event1: obj) (event2: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    subject.OnNext(event1)
    subject.OnNext(event2)
    test <@ observer.ObservedValues = [ event1 ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``emits event to observers that subscribe after completion`` (event: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    subject.OnNext(event)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [ event ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``does not complete until event is emitted from source observable`` (event: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [] @>
    test <@ not observer.ObservableCompleted @>
    subject.OnNext(event)
    test <@ observer.ObservedValues = [ event ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``completes immediately for non-empty source observable`` (event: obj) =
    let observable =
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(event)
            Action(ignore))
    let event = DerivedEvent<obj>.FromObservable(observable)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [ event ] @>
    test <@ observer.ObservableCompleted @>
