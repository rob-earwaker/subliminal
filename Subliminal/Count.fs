namespace Subliminal

open System

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
module private Increment =
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

    let private rate (bufferer: ILog<Increment> -> ILog<Buffer<Increment>>) (count: ICount) =
        count
        |> bufferer
        |> Log.map (fun buffer ->
            let total = buffer.Values |> Seq.sumBy (fun increment -> increment.Value)
            Rate(total, buffer.Interval))

    let rateByInterval interval count =
        count |> rate (Log.bufferByInterval interval)

    let rateByInterval' interval (count: ICount<'Context>) =
        count |> withoutContext |> rateByInterval interval

    let rateByBoundaries (boundaries: IObservable<'Boundary>) count =
        count |> rate (Log.bufferByBoundaries boundaries)

    let rateByBoundaries' (boundaries: IObservable<'Boundary>) (count: ICount<'Context>) =
        count |> withoutContext |> rateByBoundaries boundaries

    let subscribe onNext (count: ICount) =
        count |> Log.subscribe onNext

    let subscribe' onNext (count: ICount<'Context>) =
        count |> Log.subscribe onNext

    let subscribeForever onNext (count: ICount) =
        count |> Log.subscribeForever onNext

    let subscribeForever' onNext (count: ICount<'Context>) =
        count |> Log.subscribeForever onNext

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
