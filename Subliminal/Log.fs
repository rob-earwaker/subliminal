namespace Subliminal

open System
open System.Reactive.Linq
open System.Reactive.Subjects

/// An observable log of data.
type ILog<'Data> =
    abstract member Data : IObservable<'Data>

type Buffer<'Value> internal (values: 'Value seq, interval: TimeSpan) =
    member val Values = values
    member val Interval = interval

type Increment(value: float) =
    member val Value = value

type Increment<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type Rate(total: float, interval: TimeSpan) =
    let perSecond = lazy (total / interval.TotalSeconds)
    member val Total = total
    member val Interval = interval
    member this.PerSecond = perSecond.Value

[<RequireQualifiedAccess>]
module private Increment =
    let withoutContext (increment: Increment<'Context>) =
        Increment(increment.Value)

[<RequireQualifiedAccess>]
module Log =
    let private create data =
        { new ILog<'Data> with
            member this.Data = data }

    /// Creates a log from an observable source. This creates a subscription
    /// to the source observable that will start consuming items immediately.
    let ofObservable (observable: IObservable<'Data>) =
        // Publish the observable to ensure that all observers receive
        // the same data.
        let data = observable |> Observable.Publish
        // Connect to the published observable to start emitting data
        // from the underlying source immediately.
        data.Connect() |> ignore
        create data

    let asLog (log: ILog<'Data>) =
        create log.Data

    let asObservable (log: ILog<'Data>) =
        log.Data

    let data (log: ILog<'Data>) =
        log.Data

    let map (mapper: 'Data -> 'Mapped) (log: ILog<'Data>) =
        log.Data
        |> Observable.map mapper
        |> create

    let internal bind (binder: 'Data -> ITrigger<'Context>) (log: ILog<'Data>) =
        log.Data
        |> fun obs -> obs.SelectMany(fun data -> binder data |> Trigger.fired)
        |> create

    let count (log: ILog<'Data>) =
        log |> map (fun _ -> Increment(1.0))

    let private buffer (bufferer: IObservable<'Data> -> IObservable<#seq<'Data>>) (log: ILog<'Data>) =
        log.Data
        |> bufferer
        |> Observable.TimeInterval
        |> Observable.map (fun buffer -> Buffer<'Data>(buffer.Value, buffer.Interval))
        |> create

    let bufferByInterval (interval: TimeSpan) (log: ILog<'Data>) =
        log |> buffer (fun data -> data.Buffer(interval))

    let bufferByBoundaries (boundaries: IObservable<'Boundary>) (log: ILog<'Data>) =
        log |> buffer (fun data -> data.Buffer(boundaries))

    let private rate (bufferer: ILog<Increment> -> ILog<Buffer<Increment>>) (log: ILog<Increment>) =
        log
        |> bufferer
        |> map (fun buffer ->
            let total = buffer.Values |> Seq.sumBy (fun increment -> increment.Value)
            Rate(total, buffer.Interval))

    let rateByInterval interval log =
        log |> rate (bufferByInterval interval)

    let rateByInterval' interval (log: ILog<Increment<'Context>>) =
        log
        |> map Increment.withoutContext
        |> rateByInterval interval

    let rateByBoundaries (boundaries: IObservable<'Boundary>) log =
        log |> rate (bufferByBoundaries boundaries)

    let rateByBoundaries' (boundaries: IObservable<'Boundary>) (log: ILog<Increment<'Context>>) =
        log
        |> map Increment.withoutContext
        |> rateByBoundaries boundaries

    let subscribe onNext (log: ILog<'Data>) =
        log.Data |> Observable.subscribe onNext

    let subscribeForever onNext (log: ILog<'Data>) =
        log |> subscribe onNext |> ignore

/// A log that both captures and emits data.
type Log<'Data>
    /// A log that both captures and emits data.
    public () =
    // Synchronize the subject to ensure that data is not
    // logged at the same time and therefore that all
    // subscribers receive data in the same order.
    let subject = new Subject<'Data>() |> Subject.Synchronize

    member this.Log(data) =
        subject.OnNext(data)

    member this.Data = subject |> Observable.AsObservable

    interface ILog<'Data> with
        member this.Data = this.Data
