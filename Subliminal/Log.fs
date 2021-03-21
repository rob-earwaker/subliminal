namespace Subliminal

open System
open System.Reactive.Linq
open System.Reactive.Subjects

/// An observable log of data.
type ILog<'Data> =
    abstract member Data : IObservable<'Data>

type Rate(total: float, interval: TimeSpan) =
    let perSecond = lazy (total / interval.TotalSeconds)
    member val Total = total
    member val Interval = interval
    member this.PerSecond = perSecond.Value

/// A collection of floating point values accumulated over an interval.
type Distribution
    /// <summary>A collection of floating point values accumulated over an interval.</summary>
    /// <param name="values">The collection of values that make up the distribution.</param>
    /// <param name="interval">The interval over which values were accumulated.</param>
    (values: float seq, interval: TimeSpan) =
    let values = Array.ofSeq values
    let valuesSorted = lazy Array.sort values
    let min = lazy Array.head valuesSorted.Value
    let max = lazy Array.last valuesSorted.Value
    let mean = lazy Array.average values
    let total = lazy Array.sum values
    let rate = lazy Rate(total.Value, interval)
    let sampleRate = lazy Rate(float values.Length, interval)
    // TODO: RateOfChange

    /// The collection of values that make up the distribution.
    member val Values = values
    /// The interval over which values were accumulated.
    member val Interval = interval
    
    /// The number of values.
    member this.Count = values.Length
    /// The minimum value.
    member this.Min = min.Value
    /// The maximum value.
    member this.Max = max.Value
    /// The mean value.
    member this.Mean = mean.Value
    /// The total of all values.
    member this.Total = total.Value
    /// A rate based on the total of all values.
    member this.Rate = rate.Value
    /// A rate based on the number of values.
    member this.SampleRate = sampleRate.Value
    /// The median value.
    member this.Median = this.Quantile(0.5)

    /// <summary>Calculates a quantile of the distribution.</summary>
    /// <param name="quantile">
    /// The quantile to calculate, e.g. 0.25, 0.50 (median), 0.99.
    /// Values will be clamped to the range 0.0 <= quantile <= 1.0.
    /// </param>
    member this.Quantile(quantile) =
        let index =
            if quantile <= 0. then 0
            else if quantile >= 1. then values.Length - 1
            else int (float values.Length * quantile)
        valuesSorted.Value.[index]

/// A collection of data accumulated over an interval.
type Buffer<'Data>
    /// <summary>A collection of data accumulated over an interval.</summary>
    /// <param name="data">The collection of data items accumulated in the buffer.</param>
    /// <param name="interval">The interval over which data was accumulated.</param>
    (data: 'Data seq, interval: TimeSpan) =
    let data = Array.ofSeq data
    let dataRate = lazy Rate(float data.Length, interval)
    /// The collection of data items accumulated in the buffer.
    member val Data = data
    /// The interval over which data was accumulated.
    member val Interval = interval
    /// The number of data items.
    member val Count = data.Length
    /// A rate based on the number of data items.
    member this.DataRate = dataRate.Value

[<RequireQualifiedAccess>]
module Rate =
    let create total interval =
        Rate(total, interval)

[<RequireQualifiedAccess>]
module Distribution =
    /// <summary>A collection of floating point values accumulated over an interval.</summary>
    /// <param name="values">The collection of values that make up the distribution.</param>
    /// <param name="interval">The interval over which values were accumulated.</param>
    let create values interval =
        Distribution(values, interval)

[<RequireQualifiedAccess>]
module Buffer =
    /// <summary>A collection of data accumulated over an interval.</summary>
    /// <param name="data">The collection of data items accumulated in the buffer.</param>
    /// <param name="interval">The interval over which data was accumulated.</param>
    let create data interval =
        Buffer<'Data>(data, interval)

    /// A rate based on the number of data items.
    let dataRate (buffer: Buffer<'Data>) =
        buffer.DataRate

    let sort (buffer: Buffer<'Data>) =
        let data = buffer.Data |> Seq.sort
        create data buffer.Interval

    let sortBy (selectKey: 'Data -> 'Key) (buffer: Buffer<'Data>) =
        let data = buffer.Data |> Seq.sortBy selectKey
        create data buffer.Interval

    let groupBy (selectKey: 'Data -> 'Key) (buffer: Buffer<'Data>) =
        buffer.Data
        |> Seq.groupBy selectKey
        |> Seq.map (fun (key, data) ->
            let buffer = create data buffer.Interval
            key, buffer)

    let rate (buffer: Buffer<double>) =
        let total = Array.sum buffer.Data
        Rate.create total buffer.Interval

    let rateOf (selectIncrement: 'Data -> float) (buffer: Buffer<'Data>) =
        let total = buffer.Data |> Array.sumBy selectIncrement
        Rate.create total buffer.Interval

    let dist (buffer: Buffer<double>) =
        Distribution.create buffer.Data buffer.Interval

    let distOf (selectValue: 'Data -> float) (buffer: Buffer<'Data>) =
        let values = buffer.Data |> Seq.map selectValue
        Distribution.create values buffer.Interval

[<RequireQualifiedAccess>]
module Group =
    let key (key: 'Key, data: 'Data) =
        key

[<RequireQualifiedAccess>]
module Log =
    let private create data =
        { new ILog<'Data> with
            member this.Data = data }

    /// <summary>
    /// Creates a log from an observable source. This creates a subscription
    /// to the source observable that will start consuming items immediately.
    /// </summary>
    /// <param name="observable">The observable data source.</param>
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
        |> Observable.map (fun buffer -> Buffer.create buffer.Value buffer.Interval)
        |> create

    let bufferByInterval (interval: TimeSpan) (log: ILog<'Data>) =
        log |> buffer (fun data -> data.Buffer(interval))

    let bufferByBoundaries (boundaries: IObservable<'Boundary>) (log: ILog<'Data>) =
        log |> buffer (fun data -> data.Buffer(boundaries))

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
