namespace Subliminal

open System
open System.Diagnostics
open System.Threading.Tasks

/// An event containing information about a stopped operation.
type OperationStopped
    /// An event containing information about a stopped operation.
    internal (operationId: Guid, duration: TimeSpan, wasCanceled: bool) =
    /// An identifier for the operation.
    member val OperationId = operationId
    /// The duration of the operation.
    member val Duration = duration
    /// A flag indicating whether the operation was canceled or was completed successfully.
    member val WasCanceled = wasCanceled

/// An event containing information about a started operation.
type OperationStarted
    /// An event containing information about a started operation.
    internal (stopped: ITrigger<OperationStopped>, operationId: Guid) =
    member val internal Stopped = stopped
    /// An identifier for the operation.
    member val OperationId = operationId

/// An event containing information about a completed operation.
type OperationCompleted
    /// An event containing information about a completed operation.
    internal (operationId: Guid, duration: TimeSpan) =
    /// An identifier for the operation.
    member val OperationId = operationId
    /// The duration of the operation.
    member val Duration = duration

/// An event containing information about a canceled operation.
type OperationCanceled
    /// An event containing information about a canceled operation.
    internal (operationId: Guid, duration: TimeSpan) =
    /// An identifier for the operation.
    member val OperationId = operationId
    /// The duration of the operation.
    member val Duration = duration

/// An event containing information about a stopped operation.
type OperationStopped<'Context>
    /// An event containing information about a stopped operation.
    internal (operationId: Guid, duration: TimeSpan, wasCanceled: bool, context: 'Context) =
    /// An identifier for the operation.
    member val OperationId = operationId
    /// The duration of the operation.
    member val Duration = duration
    /// A flag indicating whether the operation was canceled or was completed successfully.
    member val WasCanceled = wasCanceled
    /// Context data associated with the operation.
    member val Context = context

/// An event containing information about a started operation.
type OperationStarted<'Context>
    /// An event containing information about a started operation.
    internal (stopped: ITrigger<OperationStopped<'Context>>, operationId: Guid, context: 'Context) =
    member val internal Stopped = stopped
    /// An identifier for the operation.
    member val OperationId = operationId
    /// Context data associated with the operation.
    member val Context = context

/// An event containing information about a completed operation.
type OperationCompleted<'Context>
    /// An event containing information about a completed operation.
    internal (operationId: Guid, duration: TimeSpan, context: 'Context) =
    /// An identifier for the operation.
    member val OperationId = operationId
    /// The duration of the operation.
    member val Duration = duration
    /// Context data associated with the operation.
    member val Context = context

/// An event containing information about a canceled operation.
type OperationCanceled<'Context>
    /// An event containing information about a canceled operation.
    internal (operationId: Guid, duration: TimeSpan, context: 'Context) =
    /// An identifier for the operation.
    member val OperationId = operationId
    /// The duration of the operation.
    member val Duration = duration
    /// Context data associated with the operation.
    member val Context = context

type IOperation =
    abstract member Started : IEvent<OperationStarted>
    abstract member Stopped : IEvent<OperationStopped>
    abstract member Completed : IEvent<OperationCompleted>
    abstract member Canceled : IEvent<OperationCanceled>

type IOperation<'Context> =
    abstract member Started : IEvent<OperationStarted<'Context>>
    abstract member Stopped : IEvent<OperationStopped<'Context>>
    abstract member Completed : IEvent<OperationCompleted<'Context>>
    abstract member Canceled : IEvent<OperationCanceled<'Context>>

[<RequireQualifiedAccess>]
module OperationStopped =
    let withoutContext (stopped: OperationStopped<'Context>) =
        OperationStopped(stopped.OperationId, stopped.Duration, stopped.WasCanceled)

    let id (stopped: OperationStopped) =
        stopped.OperationId

    let id' (stopped: OperationStopped<'Context>) =
        stopped.OperationId

    let duration (stopped: OperationStopped) =
        stopped.Duration

    let duration' (stopped: OperationStopped<'Context>) =
        stopped.Duration

    let wasCanceled (stopped: OperationStopped) =
        stopped.WasCanceled

    let wasCanceled' (stopped: OperationStopped<'Context>) =
        stopped.WasCanceled

    let context (stopped: OperationStopped<'Context>) =
        stopped.Context

[<RequireQualifiedAccess>]
module OperationStarted =
    let internal stopped' (started: OperationStarted<'Context>) =
        started.Stopped

    let internal completed' (started: OperationStarted<'Context>) =
        started.Stopped
        |> Trigger.choose (fun stopped ->
            if stopped.WasCanceled
            then None
            else Some (OperationCompleted<'Context>(stopped.OperationId, stopped.Duration, stopped.Context)))

    let internal canceled' (started: OperationStarted<'Context>) =
        started.Stopped
        |> Trigger.choose (fun stopped ->
            if stopped.WasCanceled
            then Some (OperationCanceled<'Context>(stopped.OperationId, stopped.Duration, stopped.Context))
            else None)

    let withoutContext (started: OperationStarted<'Context>) =
        let stopped = started.Stopped |> Trigger.map OperationStopped.withoutContext
        OperationStarted(stopped, started.OperationId)

    let id (started: OperationStarted) =
        started.OperationId

    let id' (started: OperationStarted<'Context>) =
        started.OperationId

    let context (started: OperationStarted<'Context>) =
        started.Context

[<RequireQualifiedAccess>]
module OperationCompleted =
    let withoutContext (completed: OperationCompleted<'Context>) =
        OperationCompleted(completed.OperationId, completed.Duration)

    let id (completed: OperationCompleted) =
        completed.OperationId

    let id' (completed: OperationCompleted<'Context>) =
        completed.OperationId

    let duration (completed: OperationCompleted) =
        completed.Duration

    let duration' (completed: OperationCompleted<'Context>) =
        completed.Duration

    let context (completed: OperationCompleted<'Context>) =
        completed.Context

[<RequireQualifiedAccess>]
module OperationCanceled =
    let withoutContext (canceled: OperationCanceled<'Context>) =
        OperationCanceled(canceled.OperationId, canceled.Duration)

    let id (canceled: OperationCanceled) =
        canceled.OperationId

    let id' (canceled: OperationCanceled<'Context>) =
        canceled.OperationId

    let duration (canceled: OperationCanceled) =
        canceled.Duration

    let duration' (canceled: OperationCanceled<'Context>) =
        canceled.Duration

    let context (canceled: OperationCanceled<'Context>) =
        canceled.Context

type internal TimerStopped(duration: TimeSpan, wasCanceled: bool) =
    member val Duration = duration
    member val WasCanceled = wasCanceled

type Timer internal () =
    let stopwatch = Stopwatch()
    let stopped = Trigger<TimerStopped>()

    member internal this.Start() =
        stopwatch.Start()

    member internal this.Stopped = stopped |> Trigger.asTrigger

    member this.Complete() =
        let wasCanceled = false
        this.Stop wasCanceled

    member this.Cancel() =
        let wasCanceled = true
        this.Stop wasCanceled

    member private this.Stop(wasCanceled) =
        if stopwatch.IsRunning then
            stopwatch.Stop()
            stopped.Raise(TimerStopped(stopwatch.Elapsed, wasCanceled))

    interface IDisposable with
        member this.Dispose() =
            this.Complete()

type Operation<'Context>() =
    let started = Event<OperationStarted<'Context>>()

    member this.StartNew(context) =
        let timer = new Timer()
        let operationId = Guid.NewGuid()
        let stopped =
            timer.Stopped
            |> Trigger.map (fun stopped ->
                OperationStopped<'Context>(operationId, stopped.Duration, stopped.WasCanceled, context))
        started.LogOccurrence(OperationStarted<'Context>(stopped, operationId, context))
        timer.Start()
        timer

    member this.Started = started |> Event.asEvent'
    member this.Stopped = started |> Log.bind OperationStarted.stopped' |> Event.ofLog'
    member this.Completed = started |> Log.bind OperationStarted.completed' |> Event.ofLog'
    member this.Canceled = started |> Log.bind OperationStarted.canceled' |> Event.ofLog'

    interface IOperation<'Context> with
        member this.Started = this.Started
        member this.Stopped = this.Stopped
        member this.Completed = this.Completed
        member this.Canceled = this.Canceled

    member this.Time(context, operation: Action<Timer>) =
        use timer = this.StartNew(context)
        operation.Invoke(timer)

    member this.Time(context, operation: Action) =
        use timer = this.StartNew(context)
        operation.Invoke()

    member this.Time(context, operation: Func<Timer, 'Result>) =
        use timer = this.StartNew(context)
        operation.Invoke(timer)

    member this.Time(context, operation: Func<'Result>) =
        use timer = this.StartNew(context)
        operation.Invoke()

    member this.AsyncTime(context, asyncOperation: Timer -> Async<'Result>) =
        async {
            use timer = this.StartNew(context)
            return! asyncOperation timer
        }

    member this.AsyncTime(context, asyncOperation: unit -> Async<'Result>) =
        async {
            use timer = this.StartNew(context)
            return! asyncOperation ()
        }

    member this.TimeAsync(context, operationAsync: Func<Timer, Task>) =
        this.AsyncTime(context, fun timer ->
            operationAsync.Invoke(timer) |> Async.AwaitTask)
        |> Async.StartAsTask
        :> Task

    member this.TimeAsync(context, operationAsync: Func<Task>) =
        this.AsyncTime(context, fun () ->
            operationAsync.Invoke() |> Async.AwaitTask)
        |> Async.StartAsTask
        :> Task

    member this.TimeAsync(context, operationAsync: Func<Timer, Task<'Result>>) =
        this.AsyncTime(context, fun timer ->
            operationAsync.Invoke(timer) |> Async.AwaitTask)
        |> Async.StartAsTask

    member this.TimeAsync(context, operationAsync: Func<Task<'Result>>) =
        this.AsyncTime(context, fun () ->
            operationAsync.Invoke() |> Async.AwaitTask)
        |> Async.StartAsTask

type Operation() =
    let operation' = Operation<unit>()

    member this.StartNew() =
        operation'.StartNew(())

    member this.Started = operation'.Started |> Log.map OperationStarted.withoutContext |> Event.ofLog'
    member this.Stopped = operation'.Stopped |> Log.map OperationStopped.withoutContext |> Event.ofLog'
    member this.Completed = operation'.Completed |> Log.map OperationCompleted.withoutContext |> Event.ofLog'
    member this.Canceled = operation'.Canceled |> Log.map OperationCanceled.withoutContext |> Event.ofLog'

    interface IOperation with
        member this.Started = this.Started
        member this.Stopped = this.Stopped
        member this.Completed = this.Completed
        member this.Canceled = this.Canceled

    member this.Time(operation: Action<Timer>) =
        operation'.Time((), operation)

    member this.Time(operation: Action) =
        operation'.Time((), operation)

    member this.Time(operation: Func<Timer, 'Result>) =
        operation'.Time((), operation)

    member this.Time(operation: Func<'Result>) =
        operation'.Time((), operation)

    member this.AsyncTime(asyncOperation: Timer -> Async<'Result>) =
        operation'.AsyncTime((), asyncOperation)

    member this.AsyncTime(asyncOperation: unit -> Async<'Result>) =
        operation'.AsyncTime((), asyncOperation)

    member this.TimeAsync(operationAsync: Func<Timer, Task>) =
        operation'.TimeAsync((), operationAsync)

    member this.TimeAsync(operationAsync: Func<Task>) =
        operation'.TimeAsync((), operationAsync)

    member this.TimeAsync(operationAsync: Func<Timer, Task<'Result>>) =
        operation'.TimeAsync((), operationAsync)

    member this.TimeAsync(operationAsync: Func<Task<'Result>>) =
        operation'.TimeAsync((), operationAsync)
