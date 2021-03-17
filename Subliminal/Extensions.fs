namespace Subliminal.Extensions

open Subliminal
open System
open System.Runtime.CompilerServices

[<Extension>]
type ObservableExtensions =
    /// Creates a log from an observable source. This creates a subscription
    /// to the source observable that will start consuming items immediately.
    [<Extension>]
    static member AsLog(observable: IObservable<'Entry>) =
        Log.ofObservable observable

[<Extension>]
type BufferExtensions =
    [<Extension>]
    static member Rate(buffer) =
        buffer |> Buffer.rate

    [<Extension>]
    static member Rate(buffer: Buffer<'Data>) =
        buffer |> Buffer.rateOfData

    [<Extension>]
    static member Rate(buffer: Buffer<'Data>, selector: Func<'Data, float>) =
        buffer |> Buffer.rateOf selector.Invoke

    [<Extension>]
    static member Distribution(buffer) =
        buffer |> Buffer.dist

    [<Extension>]
    static member Distribution(buffer: Buffer<'Data>, selector: Func<'Data, float>) =
        buffer |> Buffer.distOf selector.Invoke

[<Extension>]
type LogExtensions =
    [<Extension>]
    static member AsLog(log: ILog<'Entry>) =
        log |> Log.asLog

    [<Extension>]
    static member AsObservable(log: ILog<'Entry>) =
        log |> Log.asObservable

    [<Extension>]
    static member Select(log: ILog<'Entry>, selector: Func<'Entry, 'Mapped>) =
        log |> Log.map selector.Invoke

    [<Extension>]
    static member Buffer(log: ILog<'Entry>, interval) =
        log |> Log.bufferByInterval interval

    [<Extension>]
    static member Buffer(log: ILog<'Entry>, boundaries: IObservable<'Boundary>) =
        log |> Log.bufferByBoundaries boundaries

    [<Extension>]
    static member Rate(log) =
        log |> Log.rate

    [<Extension>]
    static member Rate(log: ILog<Buffer<'Data>>) =
        log |> Log.rateOfData

    [<Extension>]
    static member Rate(log: ILog<Buffer<'Data>>, selector: Func<'Data, float>) =
        log |> Log.rateOf selector.Invoke

    [<Extension>]
    static member Distribution(log) =
        log |> Log.dist

    [<Extension>]
    static member Distribution(log: ILog<Buffer<'Data>>, selector: Func<'Data, float>) =
        log |> Log.distOf selector.Invoke

    [<Extension>]
    static member Subscribe(log: ILog<'Entry>, onNext: Action<'Entry>) =
        log |> Log.subscribe onNext.Invoke

    [<Extension>]
    static member AsGauge(log) =
        Gauge.ofLog log

    [<Extension>]
    static member AsGauge(log: ILog<Measure<'Context>>) =
        Gauge.ofLog' log

    [<Extension>]
    static member AsCount(log) =
        Count.ofLog log

    [<Extension>]
    static member AsCount(log: ILog<Increment<'Context>>) =
        Count.ofLog' log

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
type CountExtensions =
    [<Extension>]
    static member AsCount(count) =
        count |> Count.asCount

    [<Extension>]
    static member AsCount(count: ICount<'Context>) =
        count |> Count.asCount'

    [<Extension>]
    static member WithoutContext(count: ICount<'Context>) =
        count |> Count.withoutContext

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
