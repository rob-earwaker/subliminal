﻿module Subliminal.Tests.TestOperation

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Threading.Tasks

let SleepDuration = TimeSpan.FromMilliseconds(1.)

[<Property>]
let ``ids are unique`` (context1: obj) (context2: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Started.Subscribe(observer)
    use timer1 = operation.StartNewTimer(context1)
    use timer2 = operation.StartNewTimer(context2)
    test <@ observer.ObservedValues.Length = 2 @>
    test <@ observer.ObservedValues.[0].OperationId <> observer.ObservedValues.[1].OperationId @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``id in completed operation matches started operation`` (context: obj) =
    let operation = Operation<obj>()
    let startedObserver = TestObserver()
    let completedObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use completedSubscription = operation.Completed.Subscribe(completedObserver)
    use timer = operation.StartNewTimer(context)
    timer.Complete()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ completedObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].OperationId = completedObserver.ObservedValues.[0].OperationId @>
    
[<Property>]
let ``context in completed operation matches started operation`` (context: obj) =
    let operation = Operation<obj>()
    let startedObserver = TestObserver()
    let completedObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use completedSubscription = operation.Completed.Subscribe(completedObserver)
    use timer = operation.StartNewTimer(context)
    timer.Complete()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ completedObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].Context = completedObserver.ObservedValues.[0].Context @>
    
[<Property>]
let ``id in canceled operation matches started operation`` (context: obj) =
    let operation = Operation<obj>()
    let startedObserver = TestObserver()
    let canceledObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use canceledSubscription = operation.Canceled.Subscribe(canceledObserver)
    use timer = operation.StartNewTimer(context)
    timer.Cancel()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ canceledObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].OperationId = canceledObserver.ObservedValues.[0].OperationId @>
    
[<Property>]
let ``context in canceled operation matches started operation`` (context: obj) =
    let operation = Operation<obj>()
    let startedObserver = TestObserver()
    let canceledObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use canceledSubscription = operation.Canceled.Subscribe(canceledObserver)
    use timer = operation.StartNewTimer(context)
    timer.Cancel()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ canceledObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].Context = canceledObserver.ObservedValues.[0].Context @>
    
[<Property>]
let ``id in ended operation matches started operation`` (context: obj) =
    let operation = Operation<obj>()
    let startedObserver = TestObserver()
    let endedObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use endedSubscription = operation.Ended.Subscribe(endedObserver)
    use timer = operation.StartNewTimer(context)
    timer.Complete()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ endedObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].OperationId = endedObserver.ObservedValues.[0].OperationId @>
    
[<Property>]
let ``context in ended operation matches started operation`` (context: obj) =
    let operation = Operation<obj>()
    let startedObserver = TestObserver()
    let endedObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use endedSubscription = operation.Ended.Subscribe(endedObserver)
    use timer = operation.StartNewTimer(context)
    timer.Complete()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ endedObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].Context = endedObserver.ObservedValues.[0].Context @>
    
[<Property>]
let ``emits started operation when timer started`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Started.Subscribe(observer)
    use timer = operation.StartNewTimer(context)
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ not observer.ObservableCompleted @>
        
[<Property>]
let ``emits completed operation when timer completed`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    use timer = operation.StartNewTimer(context)
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    timer.Complete()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``emits completed operation when timer disposed`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    use timer = operation.StartNewTimer(context)
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    (timer :> IDisposable).Dispose()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
        
[<Property>]
let ``emits canceled operation when timer canceled`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Canceled.Subscribe(observer)
    use timer = operation.StartNewTimer(context)
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    timer.Cancel()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``emits ended operation when timer completed`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Ended.Subscribe(observer)
    use timer = operation.StartNewTimer(context)
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    timer.Complete()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservedValues.[0].WasCanceled @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``emits ended operation when timer canceled`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Ended.Subscribe(observer)
    use timer = operation.StartNewTimer(context)
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    timer.Cancel()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ observer.ObservedValues.[0].WasCanceled @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times operation with no return value`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    operation.Time(context, fun () ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.RunSynchronously)
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times operation with return value`` (context: obj) (returnValue: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    let returnValue' = operation.Time(context, fun () ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.RunSynchronously
        returnValue)
    test <@ returnValue' = returnValue @>
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times async operation with no return value`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    operation.TimeAsync(context, fun () ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.StartAsTask
        :> Task)
    |> Async.AwaitTask
    |> Async.RunSynchronously
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times async operation with return value`` (context: obj) (returnValue: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    let returnValue' =
        operation.TimeAsync(context, fun () ->
            Async.StartAsTask <| async {
                do! Async.Sleep (int SleepDuration.TotalMilliseconds)
                return returnValue
            })
        |> Async.AwaitTask
        |> Async.RunSynchronously
    test <@ returnValue' = returnValue @>
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times operation with timer context and no return value`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    operation.Time(context, fun (timer: Timer) ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.RunSynchronously)
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times operation with timer context and return value`` (context: obj) (returnValue: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    let returnValue' = operation.Time(context, fun (timer: Timer) ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.RunSynchronously
        returnValue)
    test <@ returnValue' = returnValue @>
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times async operation with timer context and no return value`` (context: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    operation.TimeAsync(context, fun (timer: Timer) ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.StartAsTask
        :> Task)
    |> Async.AwaitTask
    |> Async.RunSynchronously
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times async operation with timer context and return value`` (context: obj) (returnValue: obj) =
    let operation = Operation<obj>()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    let returnValue' =
        operation.TimeAsync(context, fun (timer: Timer) ->
            Async.StartAsTask <| async {
                do! Async.Sleep (int SleepDuration.TotalMilliseconds)
                return returnValue
            })
        |> Async.AwaitTask
        |> Async.RunSynchronously
    test <@ returnValue' = returnValue @>
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Context = context @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    