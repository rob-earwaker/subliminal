namespace Subliminal

type ICount =
    inherit ILog<float>

type Increment<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type ICount<'Context> =
    inherit ILog<Increment<'Context>>

[<RequireQualifiedAccess>]
module Increment =
    let value (increment: Increment<'Context>) =
        increment.Value

    let context (increment: Increment<'Context>) =
        increment.Context

[<RequireQualifiedAccess>]
module Count =
    let private create increments =
        { new ICount with
            member this.Data = increments }

    let private create' increments =
        { new ICount<'Context> with
            member this.Data = increments }

    let ofLog (log: ILog<float>) =
        create log.Data

    let ofLog' (log: ILog<Increment<'Context>>) =
        create' log.Data

    let asCount (count: ICount) =
        create count.Data

    let asCount' (count: ICount<'Context>) =
        create' count.Data

    let withoutContext (count: ICount<'Context>) =
        count |> Log.map Increment.value |> ofLog

type Count<'Context>() =
    let log = Log<Increment<'Context>>()

    member this.Increment(context) =
        this.IncrementBy(1.0, context)

    member this.IncrementBy(value, context) =
        if value >= 0.0
        then log.Log(Increment<'Context>(value, context))

    member this.Data = log.Data

    interface ICount<'Context> with
        member this.Data = this.Data

type Count() =
    let log = Log<float>()

    member this.Increment() =
        this.IncrementBy(1.0)

    member this.IncrementBy(value) =
        if value >= 0.0
        then log.Log(value)

    member this.Data = log.Data

    interface ICount with
        member this.Data = this.Data
