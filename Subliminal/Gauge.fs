namespace Subliminal

type Measure(value: float) =
    member val Value = value

type Measure<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type IGauge =
    // TODO: maybe this should just be an ILog<float>
    inherit ILog<Measure>

type IGauge<'Context> =
    inherit ILog<Measure<'Context>>

[<RequireQualifiedAccess>]
module Measure =
    let value (measure: Measure) =
        measure.Value

    let value' (measure: Measure<'Context>) =
        measure.Value

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
