namespace Subliminal

type IGauge =
    inherit ILog<float>

type Measure<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type IGauge<'Context> =
    inherit ILog<Measure<'Context>>

[<RequireQualifiedAccess>]
module Measure =
    let value (measure: Measure<'Context>) =
        measure.Value

    let context (measure: Measure<'Context>) =
        measure.Context

[<RequireQualifiedAccess>]
module Gauge =
    let private create measures =
        { new IGauge with
            member this.Data = measures }

    let private create' measures =
        { new IGauge<'Context> with
            member this.Data = measures }

    let ofLog (log: ILog<float>) =
        create log.Data

    let ofLog' (log: ILog<Measure<'Context>>) =
        create' log.Data

    let asGauge (gauge: IGauge) =
        create gauge.Data

    let asGauge' (gauge: IGauge<'Context>) =
        create' gauge.Data

    let withoutContext (gauge: IGauge<'Context>) =
        gauge |> Log.map Measure.value |> ofLog

type Gauge<'Context>() =
    let log = Log<Measure<'Context>>()

    member this.LogValue(value, context) =
        log.Log(Measure<'Context>(value, context))

    member this.Data = log.Data

    interface IGauge<'Context> with
        member this.Data = this.Data

type Gauge() =
    let log = Log<float>()

    member this.LogValue(value) =
        log.Log(value)

    member this.Data = log.Data

    interface IGauge with
        member this.Data = this.Data
