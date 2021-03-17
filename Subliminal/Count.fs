namespace Subliminal

type Increment(value: float) =
    member val Value = value

type Increment<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type ICount =
    inherit ILog<Increment>

type ICount<'Context> =
    inherit ILog<Increment<'Context>>

[<RequireQualifiedAccess>]
module Increment =
    let value (increment: Increment) =
        increment.Value

    let value' (increment: Increment<'Context>) =
        increment.Value

    let withoutContext (increment: Increment<'Context>) =
        Increment(increment.Value)

[<RequireQualifiedAccess>]
module Count =
    let private create increments =
        { new ICount with
            member this.Data = increments }

    let private create' increments =
        { new ICount<'Context> with
            member this.Data = increments }

    let ofLog (log: ILog<Increment>) =
        create log.Data

    let ofLog' (log: ILog<Increment<'Context>>) =
        create' log.Data

    let asCount (count: ICount) =
        create count.Data

    let asCount' (count: ICount<'Context>) =
        create' count.Data

    let withoutContext (count: ICount<'Context>) =
        count
        |> Log.map Increment.withoutContext
        |> ofLog

type Count<'Context>() =
    let log = Log<Increment<'Context>>()
    let count = Count.ofLog' log

    member this.Increment(context) =
        this.IncrementBy(1.0, context)

    member this.IncrementBy(value, context) =
        if value >= 0.0
        then log.Log(Increment<'Context>(value, context))

    member this.Data = count.Data

    interface ICount<'Context> with
        member this.Data = this.Data

type Count() =
    let count = Count<unit>()

    member this.Increment() =
        count.Increment(())

    member this.IncrementBy(value) =
        count.IncrementBy(value, ())

    member this.Data = count |> Count.withoutContext |> Log.data

    interface ICount with
        member this.Data = this.Data
