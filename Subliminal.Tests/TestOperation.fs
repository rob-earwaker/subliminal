namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Threading
open System.Threading.Tasks

module ``Test OperationLog<TContext>`` =
    [<Property>]
    let ``ids are unique`` (context1: obj) (context2: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Started.Subscribe(observer)
        use timer1 = operation.StartNewTimer(context1)
        use timer2 = operation.StartNewTimer(context2)
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].OperationId <> observer.ObservedValues.[1].OperationId @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``id in canceled operation matches started operation`` (context: obj) =
        let operation = OperationLog<obj>()
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
        let operation = OperationLog<obj>()
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
    let ``id in completed operation matches started operation`` (context: obj) =
        let operation = OperationLog<obj>()
        let startedObserver = TestObserver()
        let completedObserver = TestObserver()
        use startedSubscription = operation.Started.Subscribe(startedObserver)
        use completedSubscription = operation.Completed.Subscribe(completedObserver)
        use timer = operation.StartNewTimer(context)
        timer.Stop()
        test <@ startedObserver.ObservedValues.Length = 1 @>
        test <@ completedObserver.ObservedValues.Length = 1 @>
        test <@ startedObserver.ObservedValues.[0].OperationId = completedObserver.ObservedValues.[0].OperationId @>
    
    [<Property>]
    let ``context in completed operation matches started operation`` (context: obj) =
        let operation = OperationLog<obj>()
        let startedObserver = TestObserver()
        let completedObserver = TestObserver()
        use startedSubscription = operation.Started.Subscribe(startedObserver)
        use completedSubscription = operation.Completed.Subscribe(completedObserver)
        use timer = operation.StartNewTimer(context)
        timer.Stop()
        test <@ startedObserver.ObservedValues.Length = 1 @>
        test <@ completedObserver.ObservedValues.Length = 1 @>
        test <@ startedObserver.ObservedValues.[0].Context = completedObserver.ObservedValues.[0].Context @>
    
    [<Property>]
    let ``emits started operation when timer started`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Started.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``emits canceled operation when timer canceled`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Canceled.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues = [] @>
        timer.Cancel()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``emits single canceled operation`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Canceled.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues = [] @>
        timer.Cancel()
        timer.Cancel()
        timer.Stop()
        (timer :> IDisposable).Dispose()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``emits completed operation when timer stopped`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues = [] @>
        Thread.Sleep(1)
        timer.Stop()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``emits completed operation when timer disposed`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues = [] @>
        Thread.Sleep(1)
        (timer :> IDisposable).Dispose()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``emits single completed operation`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues = [] @>
        Thread.Sleep(1)
        timer.Stop()
        timer.Stop()
        timer.Cancel()
        (timer :> IDisposable).Dispose()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times operation with no return value`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.Time(context, fun () -> Thread.Sleep(1))
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times operation with return value`` (context: obj) (returnValue: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' =
            operation.Time(context, fun () ->
                Thread.Sleep(1)
                returnValue)
        test <@ returnValue' = returnValue @>
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times async operation with no return value`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.TimeAsync(context, fun () -> Task.Delay(1))
        |> Async.AwaitTask
        |> Async.RunSynchronously
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times async operation with return value`` (context: obj) (returnValue: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' =
            operation.TimeAsync(context, fun () ->
                Async.StartAsTask <| async {
                    do! Async.Sleep 1
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
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.Time(context, fun (timer: Subliminal.Timer) -> Thread.Sleep(1))
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times operation with timer context and return value`` (context: obj) (returnValue: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' =
            operation.Time(context, fun (timer: Subliminal.Timer) ->
                Thread.Sleep(1)
                returnValue)
        test <@ returnValue' = returnValue @>
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times async operation with timer context and no return value`` (context: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.TimeAsync(context, fun (timer: Subliminal.Timer) -> Task.Delay(1))
        |> Async.AwaitTask
        |> Async.RunSynchronously
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times async operation with timer context and return value`` (context: obj) (returnValue: obj) =
        let operation = OperationLog<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' =
            operation.TimeAsync(context, fun (timer: Subliminal.Timer) ->
                Async.StartAsTask <| async {
                    do! Async.Sleep 1
                    return returnValue
                })
            |> Async.AwaitTask
            |> Async.RunSynchronously
        test <@ returnValue' = returnValue @>
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
module ``Test OperationLog`` =
    [<Property>]
    let ``ids are unique`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Started.Subscribe(observer)
        use timer1 = operation.StartNewTimer()
        use timer2 = operation.StartNewTimer()
        test <@ observer.ObservedValues.Length = 2 @>
        test <@ observer.ObservedValues.[0].OperationId <> observer.ObservedValues.[1].OperationId @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``id in canceled operation matches started operation`` () =
        let operation = OperationLog()
        let startedObserver = TestObserver()
        let canceledObserver = TestObserver()
        use startedSubscription = operation.Started.Subscribe(startedObserver)
        use canceledSubscription = operation.Canceled.Subscribe(canceledObserver)
        use timer = operation.StartNewTimer()
        timer.Cancel()
        test <@ startedObserver.ObservedValues.Length = 1 @>
        test <@ canceledObserver.ObservedValues.Length = 1 @>
        test <@ startedObserver.ObservedValues.[0].OperationId = canceledObserver.ObservedValues.[0].OperationId @>
        
    [<Property>]
    let ``id in completed operation matches started operation`` () =
        let operation = OperationLog()
        let startedObserver = TestObserver()
        let completedObserver = TestObserver()
        use startedSubscription = operation.Started.Subscribe(startedObserver)
        use completedSubscription = operation.Completed.Subscribe(completedObserver)
        use timer = operation.StartNewTimer()
        timer.Stop()
        test <@ startedObserver.ObservedValues.Length = 1 @>
        test <@ completedObserver.ObservedValues.Length = 1 @>
        test <@ startedObserver.ObservedValues.[0].OperationId = completedObserver.ObservedValues.[0].OperationId @>
        
    [<Property>]
    let ``emits started operation when timer started`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Started.Subscribe(observer)
        use timer = operation.StartNewTimer()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ not observer.ObservableCompleted @>
            
    [<Property>]
    let ``emits canceled operation when timer canceled`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Canceled.Subscribe(observer)
        use timer = operation.StartNewTimer()
        test <@ observer.ObservedValues = [] @>
        timer.Cancel()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``emits single canceled operation`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Canceled.Subscribe(observer)
        use timer = operation.StartNewTimer()
        test <@ observer.ObservedValues = [] @>
        timer.Cancel()
        timer.Cancel()
        timer.Stop()
        (timer :> IDisposable).Dispose()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``emits completed operation when timer stopped`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        use timer = operation.StartNewTimer()
        test <@ observer.ObservedValues = [] @>
        Thread.Sleep(1)
        timer.Stop()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``emits completed operation when timer disposed`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        use timer = operation.StartNewTimer()
        test <@ observer.ObservedValues = [] @>
        Thread.Sleep(1)
        (timer :> IDisposable).Dispose()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``emits single completed operation`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        use timer = operation.StartNewTimer()
        test <@ observer.ObservedValues = [] @>
        Thread.Sleep(1)
        timer.Stop()
        timer.Stop()
        timer.Cancel()
        (timer :> IDisposable).Dispose()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``times operation with no return value`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.Time(fun () -> Thread.Sleep(1))
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``times operation with return value`` (returnValue: obj) =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' =
            operation.Time(fun () ->
                Thread.Sleep(1)
                returnValue)
        test <@ returnValue' = returnValue @>
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``times async operation with no return value`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.TimeAsync(fun () -> Task.Delay(1))
        |> Async.AwaitTask
        |> Async.RunSynchronously
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``times async operation with return value`` (returnValue: obj) =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' =
            operation.TimeAsync(fun () ->
                Async.StartAsTask <| async {
                    do! Async.Sleep 1
                    return returnValue
                })
            |> Async.AwaitTask
            |> Async.RunSynchronously
        test <@ returnValue' = returnValue @>
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``times operation with timer context and no return value`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.Time(fun (timer: Subliminal.Timer) -> Thread.Sleep(1))
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``times operation with timer context and return value`` (returnValue: obj) =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' =
            operation.Time(fun (timer: Subliminal.Timer) ->
                Thread.Sleep(1)
                returnValue)
        test <@ returnValue' = returnValue @>
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``times async operation with timer context and no return value`` () =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.TimeAsync(fun (timer: Subliminal.Timer) -> Task.Delay(1))
        |> Async.AwaitTask
        |> Async.RunSynchronously
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``times async operation with timer context and return value`` (returnValue: obj) =
        let operation = OperationLog()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' =
            operation.TimeAsync(fun (timer: Subliminal.Timer) ->
                Async.StartAsTask <| async {
                    do! Async.Sleep 1
                    return returnValue
                })
            |> Async.AwaitTask
            |> Async.RunSynchronously
        test <@ returnValue' = returnValue @>
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
        