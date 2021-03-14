﻿namespace Subliminal

open System

type Measure(value: float) =
    member val Value = value

type Measure<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type IGauge =
    abstract member Sampled : ILog<Measure>

type IGauge<'Context> =
    abstract member Sampled : ILog<Measure<'Context>>

type Distribution(values: float seq, interval: TimeSpan) =
    let values = Array.ofSeq values
    let valuesSorted = lazy Array.sort values
    let min = lazy Array.head valuesSorted.Value
    let max = lazy Array.last valuesSorted.Value
    let mean = lazy Array.average values

    member val Values = values
    member val Interval = interval

    member this.Min = min.Value
    member this.Max = max.Value
    member this.Mean = mean.Value

[<RequireQualifiedAccess>]
module private Measure =
    let withoutContext (measure: Measure<'Context>) =
        Measure(measure.Value)

[<RequireQualifiedAccess>]
module Gauge =
    let private create sampled =
        { new IGauge with
            member this.Sampled = sampled }

    let private create' sampled =
        { new IGauge<'Context> with
            member this.Sampled = sampled }

    let ofLog log =
        create log

    let ofLog' (log: ILog<Measure<'Context>>) =
        create' log

    let asGauge (gauge: IGauge) =
        create gauge.Sampled

    let asGauge' (gauge: IGauge<'Context>) =
        create' gauge.Sampled

    let asLog (gauge: IGauge) =
        gauge.Sampled

    let asLog' (gauge: IGauge<'Context>) =
        gauge.Sampled

    let asObservable gauge =
        gauge |> asLog |> Log.asObservable

    let asObservable' (gauge: IGauge<'Context>) =
        gauge |> asLog' |> Log.asObservable

    let sampled (gauge: IGauge) =
        gauge.Sampled

    let sampled' (gauge: IGauge<'Context>) =
        gauge.Sampled

    let withoutContext (gauge: IGauge<'Context>) =
        gauge.Sampled
        |> Log.map Measure.withoutContext
        |> create

    let private dist (bufferer: ILog<Measure> -> ILog<Buffer<Measure>>) (gauge: IGauge) =
        gauge.Sampled
        |> bufferer
        |> Log.map (fun buffer ->
            let values = buffer.Values |> Seq.map (fun measure -> measure.Value) |> Seq.cache
            Distribution(values, buffer.Interval))

    let distByInterval interval gauge =
        gauge |> dist (Log.bufferByInterval interval)

    let distByInterval' interval (gauge: IGauge<'Context>) =
        gauge |> withoutContext |> distByInterval interval

    let distByBoundaries (boundaries: IObservable<'Boundary>) gauge =
        gauge |> dist (Log.bufferByBoundaries boundaries)

    let distByBoundaries' (boundaries: IObservable<'Boundary>) (gauge: IGauge<'Context>) =
        gauge |> withoutContext |> distByBoundaries boundaries

    let subscribe onNext (gauge: IGauge) =
        gauge.Sampled |> Log.subscribe onNext

    let subscribe' onNext (gauge: IGauge<'Context>) =
        gauge.Sampled |> Log.subscribe onNext

    let subscribeForever onNext (gauge: IGauge) =
        gauge.Sampled |> Log.subscribeForever onNext

    let subscribeForever' onNext (gauge: IGauge<'Context>) =
        gauge.Sampled |> Log.subscribeForever onNext

type Gauge<'Context>() =
    let sampled = Log<Measure<'Context>>()
    let gauge = Gauge.ofLog' sampled

    member this.LogValue(value, context) =
        sampled.LogEntry(Measure<'Context>(value, context))

    member this.Sampled = gauge.Sampled

    interface IGauge<'Context> with
        member this.Sampled = this.Sampled

type Gauge() =
    let gauge = Gauge<unit>()

    member this.LogValue(value) =
        gauge.LogValue(value, ())

    member this.Sampled = gauge |> Gauge.withoutContext |> Gauge.sampled

    interface IGauge with
        member this.Sampled = this.Sampled
