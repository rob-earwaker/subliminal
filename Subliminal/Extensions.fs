namespace Subliminal

open Subliminal
open System
open System.Runtime.CompilerServices

type Group<'Key, 'Data> internal (key: 'Key, data: 'Data seq, interval: TimeSpan) =
    let data = Array.ofSeq data
    member val Key = key
    member val Data = data
    member val Interval = interval

[<Extension>]
type ObservableExtensions =
    /// Creates a log from an observable source. This creates a subscription
    /// to the source observable that will start consuming items immediately.
    [<Extension>]
    static member AsLog(observable: IObservable<'Data>) =
        Log.ofObservable observable

[<Extension>]
type BufferExtensions =
    [<Extension>]
    static member Order(buffer: Buffer<'Data>) =
        buffer |> Buffer.sort

    [<Extension>]
    static member OrderBy(buffer, keySelector: Func<'Data, 'Key>) =
        buffer |> Buffer.sortBy keySelector.Invoke

    [<Extension>]
    static member GroupBy(buffer, keySelector: Func<'Data, 'Key>) =
        buffer
        |> Buffer.groupBy keySelector.Invoke
        |> Seq.map (fun (key, buffer) ->
            Group<'Key, 'Data>(key, buffer.Data, buffer.Interval))

    [<Extension>]
    static member Rate(buffer) =
        buffer |> Buffer.rate

    [<Extension>]
    static member Rate(buffer: Buffer<'Data>, incrementSelector: Func<'Data, float>) =
        buffer |> Buffer.rateOf incrementSelector.Invoke

    [<Extension>]
    static member Distribution(buffer) =
        buffer |> Buffer.dist

    [<Extension>]
    static member Distribution(buffer: Buffer<'Data>, valueSelector: Func<'Data, float>) =
        buffer |> Buffer.distOf valueSelector.Invoke

[<Extension>]
type LogExtensions =
    [<Extension>]
    static member AsLog(log: ILog<'Data>) =
        log |> Log.asLog

    [<Extension>]
    static member AsObservable(log: ILog<'Data>) =
        log |> Log.asObservable

    [<Extension>]
    static member Select(log: ILog<'Data>, selector: Func<'Data, 'Mapped>) =
        log |> Log.map selector.Invoke

    [<Extension>]
    static member SelectMany(log: ILog<'Data>, selector: Func<'Data, #seq<'Mapped>>) =
        log |> Log.collect selector.Invoke

    [<Extension>]
    static member Concat(log: ILog<#seq<'Data>>) =
        log |> Log.concat

    [<Extension>]
    static member Buffer(log: ILog<'Data>, interval) =
        log |> Log.bufferByInterval interval

    [<Extension>]
    static member Buffer(log: ILog<'Data>, boundaries: IObservable<'Boundary>) =
        log |> Log.bufferByBoundaries boundaries

    [<Extension>]
    static member Subscribe(log: ILog<'Data>, onNext: Action<'Data>) =
        log |> Log.subscribe onNext.Invoke

    [<Extension>]
    static member AsGauge(log) =
        Gauge.ofLog log

    [<Extension>]
    static member AsGauge(log: ILog<Measure<'Context>>) =
        Gauge.ofLog' log

    [<Extension>]
    static member AsCounter(log) =
        Counter.ofLog log

    [<Extension>]
    static member AsCounter(log: ILog<Increment<'Context>>) =
        Counter.ofLog' log

    [<Extension>]
    static member AsEvent(log) =
        Event.ofLog log

    [<Extension>]
    static member AsEvent(log: ILog<'Context>) =
        Event.ofLog' log

[<Extension>]
type GaugeExtensions =
    [<Extension>]
    static member AsGauge(gauge) =
        gauge |> Gauge.asGauge

    [<Extension>]
    static member AsGauge(gauge: IGauge<'Context>) =
        gauge |> Gauge.asGauge'

    [<Extension>]
    static member WithoutContext(gauge: IGauge<'Context>) =
        gauge |> Gauge.withoutContext

[<Extension>]
type CounterExtensions =
    [<Extension>]
    static member AsCounter(counter) =
        counter |> Counter.asCounter

    [<Extension>]
    static member AsCounter(counter: ICounter<'Context>) =
        counter |> Counter.asCounter'

    [<Extension>]
    static member WithoutContext(counter: ICounter<'Context>) =
        counter |> Counter.withoutContext

[<Extension>]
type EventExtensions =
    [<Extension>]
    static member AsEvent(event) =
        event |> Event.asEvent

    [<Extension>]
    static member AsEvent(event: IEvent<'Event>) =
        event |> Event.asEvent'

    [<Extension>]
    static member WithoutContext(event: IEvent<'Event>) =
        event |> Event.withoutContext
