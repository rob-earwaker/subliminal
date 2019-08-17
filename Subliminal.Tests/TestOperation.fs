namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Threading.Tasks

module ``Test Operation<TContext>`` =
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
    let ``id in completed operation matches started operation`` (context: obj) =
        let operation = Operation<obj>()
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
        let operation = Operation<obj>()
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
        let operation = Operation<obj>()
        let observer = TestObserver()
        use subscription = operation.Started.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``emits canceled operation when timer canceled`` (context: obj) =
        let operation = Operation<obj>()
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
        let operation = Operation<obj>()
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
        let operation = Operation<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues = [] @>
        timer.Stop()
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
        (timer :> IDisposable).Dispose()
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``emits single completed operation`` (context: obj) =
        let operation = Operation<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        use timer = operation.StartNewTimer(context)
        test <@ observer.ObservedValues = [] @>
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
        let operation = Operation<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        operation.Time(context, fun () -> ())
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times operation with return value`` (context: obj) (returnValue: obj) =
        let operation = Operation<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' = operation.Time(context, fun () -> returnValue)
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
        operation.TimeAsync(context, fun () -> Task.CompletedTask)
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
            operation.TimeAsync(context, fun () -> Task.FromResult(returnValue))
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
        operation.Time(context, fun (timer: Timer) -> ())
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    
    [<Property>]
    let ``times operation with timer context and return value`` (context: obj) (returnValue: obj) =
        let operation = Operation<obj>()
        let observer = TestObserver()
        use subscription = operation.Completed.Subscribe(observer)
        let returnValue' = operation.Time(context, fun (timer: Timer) -> returnValue)
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
        operation.TimeAsync(context, fun (timer: Timer) -> Task.CompletedTask)
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
            operation.TimeAsync(context, fun (timer: Timer) -> Task.FromResult(returnValue))
            |> Async.AwaitTask
            |> Async.RunSynchronously
        test <@ returnValue' = returnValue @>
        test <@ observer.ObservedValues.Length = 1 @>
        test <@ observer.ObservedValues.[0].Context = context @>
        test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
        test <@ not observer.ObservableCompleted @>
    