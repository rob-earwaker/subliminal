namespace Subliminal

open System

type Increment(value: float) =
    member val Value = value

type Increment<'Context>(value: float, context: 'Context) =
    member val Value = value
    member val Context = context

type ICount =
    abstract member Incremented : ILog<Increment>

type ICount<'Context> =
    abstract member Incremented : ILog<Increment<'Context>>

type Rate(delta: float, interval: TimeSpan) =
    let deltaPerSecond = lazy(delta / interval.TotalSeconds)
    member val Delta = delta
    member val Interval = interval
    member this.DeltaPerSecond = deltaPerSecond.Value

[<RequireQualifiedAccess>]
module private Increment =
    let withoutContext (increment: Increment<'Context>) =
        Increment(increment.Value)

[<RequireQualifiedAccess>]
module Count =
    let private create incremented =
        { new ICount with
            member this.Incremented = incremented }

    let private create' incremented =
        { new ICount<'Context> with
            member this.Incremented = incremented }

    let ofLog log =
        create log

    let ofLog' (log: ILog<Increment<'Context>>) =
        create' log

    let asCount (count: ICount) =
        create count.Incremented

    let asCount' (count: ICount<'Context>) =
        create' count.Incremented

    let asLog (count: ICount) =
        count.Incremented

    let asLog' (count: ICount<'Context>) =
        count.Incremented

    let asObservable count =
        count |> asLog |> Log.asObservable

    let asObservable' (count: ICount<'Context>) =
        count |> asLog' |> Log.asObservable

    let incremented (count: ICount) =
        count.Incremented

    let incremented' (count: ICount<'Context>) =
        count.Incremented

    let withoutContext (count: ICount<'Context>) =
        count.Incremented
        |> Log.map Increment.withoutContext
        |> create

    let private rate (bufferer: ILog<Increment> -> ILog<Buffer<Increment>>) (count: ICount) =
        count.Incremented
        |> bufferer
        |> Log.map (fun buffer ->
            let delta = buffer.Values |> Seq.sumBy (fun increment -> increment.Value)
            Rate(delta, buffer.Interval))

    let rateByInterval interval count =
        count |> rate (Log.bufferByInterval interval)

    let rateByInterval' interval (count: ICount<'Context>) =
        count |> withoutContext |> rateByInterval interval

    let rateByBoundaries (boundaries: IObservable<'Boundary>) count =
        count |> rate (Log.bufferByBoundaries boundaries)

    let rateByBoundaries' (boundaries: IObservable<'Boundary>) (count: ICount<'Context>) =
        count |> withoutContext |> rateByBoundaries boundaries

    let subscribe onNext (count: ICount) =
        count.Incremented |> Log.subscribe onNext

    let subscribe' onNext (count: ICount<'Context>) =
        count.Incremented |> Log.subscribe onNext

    let subscribeForever onNext (count: ICount) =
        count.Incremented |> Log.subscribeForever onNext

    let subscribeForever' onNext (count: ICount<'Context>) =
        count.Incremented |> Log.subscribeForever onNext

type Count<'Context>() =
    let incremented = Log<Increment<'Context>>()
    let count = Count.ofLog' incremented

    member this.Increment(context) =
        this.IncrementBy(1.0, context)

    member this.IncrementBy(value, context) =
        if value >= 0.0
        then incremented.LogEntry(Increment<'Context>(value, context))

    member this.Incremented = count.Incremented

    interface ICount<'Context> with
        member this.Incremented = this.Incremented

type Count() =
    let count = Count<unit>()

    member this.Increment() =
        count.Increment(())

    member this.IncrementBy(value) =
        count.IncrementBy(value, ())

    member this.Incremented = count |> Count.withoutContext |> Count.incremented

    interface ICount with
        member this.Incremented = this.Incremented
