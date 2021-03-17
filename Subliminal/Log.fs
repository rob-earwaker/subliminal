namespace Subliminal

open System
open System.Reactive.Linq
open System.Reactive.Subjects

/// An observable log of data.
type ILog<'Data> =
    abstract member Data : IObservable<'Data>

type Buffer<'Data> internal (data: 'Data seq, interval: TimeSpan) =
    let data = Array.ofSeq data
    member val Data = data
    member val Interval = interval
    member val Count = data.Length

type Rate(total: float, interval: TimeSpan) =
    let perSecond = lazy (total / interval.TotalSeconds)
    member val Total = total
    member val Interval = interval
    member this.PerSecond = perSecond.Value

type Distribution(values: float seq, interval: TimeSpan) =
    let values = Array.ofSeq values
    let valuesSorted = lazy Array.sort values
    let min = lazy Array.head valuesSorted.Value
    let max = lazy Array.last valuesSorted.Value
    let mean = lazy Array.average values
    let total = lazy Array.sum values
    let rate = lazy Rate(total.Value, interval)
    // TODO: RateOfChange
    // TODO: Median, Percentile(), Percentile99, Percentile50, Percentile90, Percentile05
    // TODO: Should Rate and/or RateOfChange be a Distribution?

    member val Values = values
    member val Interval = interval

    member this.Min = min.Value
    member this.Max = max.Value
    member this.Mean = mean.Value
    member this.Total = total.Value
    member this.Rate = rate.Value

[<RequireQualifiedAccess>]
module Buffer =
    let sort (buffer: Buffer<'Data>) =
        let data = buffer.Data |> Seq.sort
        Buffer<'Data>(data, buffer.Interval)

    let sortBy (selectKey: 'Data -> 'Key) (buffer: Buffer<'Data>) =
        let data = buffer.Data |> Seq.sortBy selectKey
        Buffer<'Data>(data, buffer.Interval)

    let groupBy (selectKey: 'Data -> 'Key) (buffer: Buffer<'Data>) =
        buffer.Data
        |> Array.groupBy selectKey
        |> Array.map (fun (key, data) ->
            let buffer = Buffer<'Data>(data, buffer.Interval)
            key, buffer)

    let rate (buffer: Buffer<double>) =
        let total = Array.sum buffer.Data
        Rate(total, buffer.Interval)

    let rateOfData (buffer: Buffer<'Data>) =
        let total = float buffer.Count
        Rate(total, buffer.Interval)

    let rateOf (selectIncrement: 'Data -> float) (buffer: Buffer<'Data>) =
        let total = buffer.Data |> Array.sumBy selectIncrement
        Rate(total, buffer.Interval)

    let dist (buffer: Buffer<double>) =
        Distribution(buffer.Data, buffer.Interval)

    let distOf (selectSample: 'Data -> float) (buffer: Buffer<'Data>) =
        let values = buffer.Data |> Seq.map selectSample
        Distribution(values, buffer.Interval)

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

    let map (mapping: 'Data -> 'Mapped) (log: ILog<'Data>) =
        log.Data
        |> Observable.map mapping
        |> create

    let collect (mapping: 'Data -> #seq<'Mapped>) (log: ILog<'Data>) =
        log.Data
        |> fun obs -> obs.SelectMany(fun data -> mapping data :> seq<'Mapped>)
        |> create

    let concat (log: ILog<#seq<'Data>>) =
        log |> collect id

    let internal bind (binder: 'Data -> ITrigger<'Context>) (log: ILog<'Data>) =
        log.Data
        |> fun obs -> obs.SelectMany(fun data -> binder data |> Trigger.fired)
        |> create

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

    let groupBy (selectKey: 'Data -> 'Key) (log: ILog<Buffer<'Data>>) =
        log |> collect (Buffer.groupBy selectKey)

    let rate (log: ILog<Buffer<float>>) =
        log |> map Buffer.rate

    let rateOfData (log: ILog<Buffer<'Data>>) =
        log |> map Buffer.rateOfData

    let rateOf selectIncrement (log: ILog<Buffer<'Data>>) =
        log |> map (Buffer.rateOf selectIncrement)

    let dist (log: ILog<Buffer<float>>) =
        log |> map Buffer.dist

    let distOf selectSample (log: ILog<Buffer<'Data>>) =
        log |> map (Buffer.distOf selectSample)

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
