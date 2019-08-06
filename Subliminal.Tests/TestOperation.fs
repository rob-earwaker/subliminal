module Subliminal.Tests.TestOperation

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Threading.Tasks
open Xunit

let SleepDuration = TimeSpan.FromMilliseconds(1.)

[<Fact>]
let ``ids are unique`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Started.Subscribe(observer)
    use timer1 = operation.StartNewTimer()
    use timer2 = operation.StartNewTimer()
    test <@ observer.ObservedValues.Length = 2 @>
    test <@ observer.ObservedValues.[0].OperationId <> observer.ObservedValues.[1].OperationId @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``id in completed operation matches started operation`` () =
    let operation = Operation()
    let startedObserver = TestObserver()
    let completedObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use completedSubscription = operation.Completed.Subscribe(completedObserver)
    use timer = operation.StartNewTimer()
    timer.Complete()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ completedObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].OperationId = completedObserver.ObservedValues.[0].OperationId @>
    
[<Fact>]
let ``id in canceled operation matches started operation`` () =
    let operation = Operation()
    let startedObserver = TestObserver()
    let canceledObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use canceledSubscription = operation.Canceled.Subscribe(canceledObserver)
    use timer = operation.StartNewTimer()
    timer.Cancel()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ canceledObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].OperationId = canceledObserver.ObservedValues.[0].OperationId @>
    
[<Fact>]
let ``id in ended operation matches started operation`` () =
    let operation = Operation()
    let startedObserver = TestObserver()
    let endedObserver = TestObserver()
    use startedSubscription = operation.Started.Subscribe(startedObserver)
    use endedSubscription = operation.Ended.Subscribe(endedObserver)
    use timer = operation.StartNewTimer()
    timer.Complete()
    test <@ startedObserver.ObservedValues.Length = 1 @>
    test <@ endedObserver.ObservedValues.Length = 1 @>
    test <@ startedObserver.ObservedValues.[0].OperationId = endedObserver.ObservedValues.[0].OperationId @>
    
[<Fact>]
let ``emits started operation when timer started`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Started.Subscribe(observer)
    use timer = operation.StartNewTimer()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ not observer.ObservableCompleted @>
        
[<Fact>]
let ``emits completed operation when timer completed`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    use timer = operation.StartNewTimer()
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    timer.Complete()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``emits completed operation when timer disposed`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    use timer = operation.StartNewTimer()
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    (timer :> IDisposable).Dispose()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
        
[<Fact>]
let ``emits canceled operation when timer canceled`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Canceled.Subscribe(observer)
    use timer = operation.StartNewTimer()
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    timer.Cancel()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``emits ended operation when timer completed`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Ended.Subscribe(observer)
    use timer = operation.StartNewTimer()
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    timer.Complete()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservedValues.[0].WasCanceled @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``emits ended operation when timer canceled`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Ended.Subscribe(observer)
    use timer = operation.StartNewTimer()
    test <@ observer.ObservedValues = [] @>
    Async.Sleep (int SleepDuration.TotalMilliseconds)
    |> Async.RunSynchronously
    timer.Cancel()
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ observer.ObservedValues.[0].WasCanceled @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``times operation with no return value`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    operation.Time(fun () ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.RunSynchronously)
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times operation with return value`` (returnValue: obj) =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    let returnValue' = operation.Time(fun () ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.RunSynchronously
        returnValue)
    test <@ returnValue' = returnValue @>
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``times async operation with no return value`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    operation.TimeAsync(fun () ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.StartAsTask
        :> Task)
    |> Async.AwaitTask
    |> Async.RunSynchronously
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times async operation with return value`` (returnValue: obj) =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    let returnValue' =
        operation.TimeAsync(fun () ->
            Async.StartAsTask <| async {
                do! Async.Sleep (int SleepDuration.TotalMilliseconds)
                return returnValue
            })
        |> Async.AwaitTask
        |> Async.RunSynchronously
    test <@ returnValue' = returnValue @>
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``times operation with timer context and no return value`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    operation.Time(fun (timer: Timer) ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.RunSynchronously)
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times operation with timer context and return value`` (returnValue: obj) =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    let returnValue' = operation.Time(fun (timer: Timer) ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.RunSynchronously
        returnValue)
    test <@ returnValue' = returnValue @>
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Fact>]
let ``times async operation with timer context and no return value`` () =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    operation.TimeAsync(fun (timer: Timer) ->
        Async.Sleep (int SleepDuration.TotalMilliseconds)
        |> Async.StartAsTask
        :> Task)
    |> Async.AwaitTask
    |> Async.RunSynchronously
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    
[<Property>]
let ``times async operation with timer context and return value`` (returnValue: obj) =
    let operation = Operation()
    let observer = TestObserver()
    use subscription = operation.Completed.Subscribe(observer)
    let returnValue' =
        operation.TimeAsync(fun (timer: Timer) ->
            Async.StartAsTask <| async {
                do! Async.Sleep (int SleepDuration.TotalMilliseconds)
                return returnValue
            })
        |> Async.AwaitTask
        |> Async.RunSynchronously
    test <@ returnValue' = returnValue @>
    test <@ observer.ObservedValues.Length = 1 @>
    test <@ observer.ObservedValues.[0].Duration > TimeSpan.Zero @>
    test <@ not observer.ObservableCompleted @>
    