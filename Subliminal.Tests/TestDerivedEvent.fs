module Subliminal.Tests.TestDerivedEvent

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Reactive.Linq
open System.Reactive.Subjects

[<Property>]
let ``emits first context value from source observable`` (context1: obj) (context2: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    subject.OnNext(context1)
    subject.OnNext(context2)
    test <@ observer.ObservedValues = [ context1 ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``emits context value to observers that subscribe after completion`` (context: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    subject.OnNext(context)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [ context ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``does not complete until context value is emitted from source observable`` (context: obj) =
    use subject = new Subject<obj>()
    let event = DerivedEvent<obj>.FromObservable(subject)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [] @>
    test <@ not observer.ObservableCompleted @>
    subject.OnNext(context)
    test <@ observer.ObservedValues = [ context ] @>
    test <@ observer.ObservableCompleted @>
    
[<Property>]
let ``completes immediately for non-empty source observable`` (context: obj) =
    let observable =
        Observable.Create<obj>(fun (observer: IObserver<obj>) ->
            observer.OnNext(context)
            Action(ignore))
    let event = DerivedEvent<obj>.FromObservable(observable)
    let observer = TestObserver()
    use subscription = event.Subscribe(observer)
    test <@ observer.ObservedValues = [ context ] @>
    test <@ observer.ObservableCompleted @>
