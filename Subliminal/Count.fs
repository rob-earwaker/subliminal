namespace Subliminal

type ICounter =
    inherit ILog<float>

type Increment<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type ICounter<'Context> =
    inherit ILog<Increment<'Context>>

[<RequireQualifiedAccess>]
module Increment =
    let value (increment: Increment<'Context>) =
        increment.Value

    let context (increment: Increment<'Context>) =
        increment.Context

[<RequireQualifiedAccess>]
module Counter =
    let private create increments =
        { new ICounter with
            member this.Data = increments }

    let private create' increments =
        { new ICounter<'Context> with
            member this.Data = increments }

    let ofLog (log: ILog<float>) =
        create log.Data

    let ofLog' (log: ILog<Increment<'Context>>) =
        create' log.Data

    let asCounter (counter: ICounter) =
        create counter.Data

    let asCounter' (counter: ICounter<'Context>) =
        create' counter.Data

    let withoutContext (counter: ICounter<'Context>) =
        counter |> Log.map Increment.value |> ofLog

type Counter<'Context>() =
    let log = Log<Increment<'Context>>()

    member this.Increment(context) =
        this.IncrementBy(1.0, context)

    member this.IncrementBy(value, context) =
        if value >= 0.0
        then log.Log(Increment<'Context>(value, context))

    member this.Data = log.Data

    interface ICounter<'Context> with
        member this.Data = this.Data

type Counter() =
    let log = Log<float>()

    member this.Increment() =
        this.IncrementBy(1.0)

    member this.IncrementBy(value) =
        if value >= 0.0
        then log.Log(value)

    member this.Data = log.Data

    interface ICounter with
        member this.Data = this.Data
