module Subliminal.Tests.TestDerivedEvent

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive.Linq
open System.Reactive.Subjects

[<Property>]
let ``emits first event from source observable`` (eventValue1: obj) (eventValue2: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    subject.OnNext(eventValue1)
    subject.OnNext(eventValue2)
    test <@ observer.ObservedValues = [ eventValue1 ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``emits event to observers that subscribe after completion`` (eventValue: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    subject.OnNext(eventValue)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [ eventValue ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``does not complete until event is emitted from source observable`` (eventValue: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [] @>
    test <@ not observer.ObservableCompleted @>
    subject.OnNext(eventValue)
    test <@ observer.ObservedValues = [ eventValue ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``completes immediately for non-empty source observable`` (eventValue: obj) =
    let observable =
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(eventValue)
            Action(ignore))
    let event = DerivedEvent<obj>.FromObservable(observable)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [ eventValue ] @>
    test <@ observer.ObservableCompleted @>
