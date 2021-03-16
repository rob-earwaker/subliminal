namespace Subliminal

open System

type Measure(value: float) =
    member val Value = value

type Measure<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type IGauge =
    inherit ILog<Measure>

type IGauge<'Context> =
    inherit ILog<Measure<'Context>>

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
module private Measure =
    let withoutContext (measure: Measure<'Context>) =
        Measure(measure.Value)

[<RequireQualifiedAccess>]
module Gauge =
    let private create measures =
        { new IGauge with
            member this.Data = measures }

    let private create' measures =
        { new IGauge<'Context> with
            member this.Data = measures }

    let ofLog (log: ILog<Measure>) =
        create log.Data

    let ofLog' (log: ILog<Measure<'Context>>) =
        create' log.Data

    let asGauge (gauge: IGauge) =
        create gauge.Data

    let asGauge' (gauge: IGauge<'Context>) =
        create' gauge.Data

    let withoutContext (gauge: IGauge<'Context>) =
        gauge
        |> Log.map Measure.withoutContext
        |> ofLog

    let private dist (bufferer: ILog<Measure> -> ILog<Buffer<Measure>>) (gauge: IGauge) =
        gauge
        |> bufferer
        |> Log.map (fun buffer ->
            let values = buffer.Values |> Seq.map (fun measure -> measure.Value)
            Distribution(values, buffer.Interval))

    let distByInterval interval gauge =
        gauge |> dist (Log.bufferByInterval interval)

    let distByInterval' interval (gauge: IGauge<'Context>) =
        gauge |> withoutContext |> distByInterval interval

    let distByBoundaries (boundaries: IObservable<'Boundary>) gauge =
        gauge |> dist (Log.bufferByBoundaries boundaries)

    let distByBoundaries' (boundaries: IObservable<'Boundary>) (gauge: IGauge<'Context>) =
        gauge |> withoutContext |> distByBoundaries boundaries

[<RequireQualifiedAccess>]
module Distribution =
    let ofBuffer (buffer: Buffer<float>) =
        Distribution(buffer.Values, buffer.Interval)

    let ofBuffer' (mapper: 'Value -> float) (buffer: Buffer<'Value>) =
        let values = buffer.Values |> Seq.map mapper
        Distribution(values, buffer.Interval)

type Gauge<'Context>() =
    let log = Log<Measure<'Context>>()
    let gauge = Gauge.ofLog' log

    member this.LogValue(value, context) =
        log.Log(Measure<'Context>(value, context))

    member this.Data = gauge.Data

    interface IGauge<'Context> with
        member this.Data = this.Data

type Gauge() =
    let gauge = Gauge<unit>()

    member this.LogValue(value) =
        gauge.LogValue(value, ())

    member this.Data = gauge |> Gauge.withoutContext |> Log.data

    interface IGauge with
        member this.Data = this.Data
