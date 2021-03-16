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
    static member Distribution(gauge, interval) =
        gauge |> Gauge.distByInterval interval

    [<Extension>]
    static member Distribution(gauge: IGauge<'Context>, interval) =
        gauge |> Gauge.distByInterval' interval

    [<Extension>]
    static member Distribution(gauge, boundaries: IObservable<'Boundary>) =
        gauge |> Gauge.distByBoundaries boundaries

    [<Extension>]
    static member Distribution(gauge: IGauge<'Context>, boundaries: IObservable<'Boundary>) =
        gauge |> Gauge.distByBoundaries' boundaries

    [<Extension>]
    static member Subscribe(gauge, onNext: Action<Measure>) =
        gauge |> Gauge.subscribe onNext.Invoke

    [<Extension>]
    static member Subscribe(gauge: IGauge<'Context>, onNext: Action<Measure<'Context>>) =
        gauge |> Gauge.subscribe' onNext.Invoke

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
    static member Rate(count, interval) =
        count |> Count.rateByInterval interval

    [<Extension>]
    static member Rate(count: ICount<'Context>, interval) =
        count |> Count.rateByInterval' interval

    [<Extension>]
    static member Rate(count, boundaries: IObservable<'Boundary>) =
        count |> Count.rateByBoundaries boundaries

    [<Extension>]
    static member Rate(count: ICount<'Context>, boundaries: IObservable<'Boundary>) =
        count |> Count.rateByBoundaries' boundaries

    [<Extension>]
    static member Subscribe(count, onNext: Action<Increment>) =
        count |> Count.subscribe onNext.Invoke

    [<Extension>]
    static member Subscribe(count: ICount<'Context>, onNext: Action<Increment<'Context>>) =
        count |> Count.subscribe' onNext.Invoke

[<Extension>]
type EventExtensions =
    [<Extension>]
    static member AsEvent(event) =
        event |> Event.asEvent

    [<Extension>]
    static member AsEvent(event: IEvent<'Event>) =
        event |> Event.asEvent'

    [<Extension>]
    static member AsLog(event) =
        event |> Event.asLog

    [<Extension>]
    static member AsLog(event: IEvent<'Event>) =
        event |> Event.asLog'

    [<Extension>]
    static member AsObservable(event) =
        event |> Event.asObservable

    [<Extension>]
    static member AsObservable(event: IEvent<'Event>) =
        event |> Event.asObservable'

    [<Extension>]
    static member WithoutContext(event: IEvent<'Event>) =
        event |> Event.withoutContext

    [<Extension>]
    static member Select(event, selector: Func<'Mapped>) =
        event |> Event.map selector.Invoke

    [<Extension>]
    static member Select(event: IEvent<'Event>, selector: Func<'Event, 'Mapped>) =
        event |> Event.map' selector.Invoke

    [<Extension>]
    static member Count(event) =
        event |> Event.count

    [<Extension>]
    static member Count(event: IEvent<'Event>) =
        event |> Event.count'

    [<Extension>]
    static member Buffer(event: IEvent<'Event>, interval) =
        event |> Event.bufferByInterval' interval

    [<Extension>]
    static member Buffer(event: IEvent<'Event>, boundaries: IObservable<'Boundary>) =
        event |> Event.bufferByBoundaries' boundaries

    [<Extension>]
    static member Rate(event, interval) =
        event |> Event.rateByInterval interval

    [<Extension>]
    static member Rate(event: IEvent<'Event>, interval) =
        event |> Event.rateByInterval' interval

    [<Extension>]
    static member Rate(event, boundaries: IObservable<'Boundary>) =
        event |> Event.rateByBoundaries boundaries

    [<Extension>]
    static member Rate(event: IEvent<'Event>, boundaries: IObservable<'Boundary>) =
        event |> Event.rateByBoundaries' boundaries

    [<Extension>]
    static member Subscribe(event, onNext: Action) =
        event |> Event.subscribe onNext.Invoke

    [<Extension>]
    static member Subscribe(event: IEvent<'Event>, onNext: Action<'Event>) =
        event |> Event.subscribe' onNext.Invoke
