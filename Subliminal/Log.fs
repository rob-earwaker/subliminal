namespace Subliminal

open System
open System.Reactive.Linq
open System.Reactive.Subjects

/// An observable log of entries.
type ILog<'Entry> =
    abstract member EntryLogged : IObservable<'Entry>

type Buffer<'Value> internal (values: 'Value seq, interval: TimeSpan) =
    member val Values = values
    member val Interval = interval

[<RequireQualifiedAccess>]
module Log =
    let private create entryLogged =
        { new ILog<'Entry> with
            member this.EntryLogged = entryLogged }

    /// Creates a log from an observable source. This creates a subscription
    /// to the source observable that will start consuming items immediately.
    let ofObservable (observable: IObservable<'Entry>) =
        // Publish the observable to ensure that all observers receive
        // the same entries.
        let entryLogged = observable |> Observable.Publish
        // Connect to the published observable to start emitting entries
        // from the underlying source immediately.
        entryLogged.Connect() |> ignore
        create entryLogged

    let asLog (log: ILog<'Entry>) =
        create log.EntryLogged

    let asObservable (log: ILog<'Entry>) =
        log.EntryLogged

    let entryLogged (log: ILog<'Entry>) =
        log.EntryLogged

    let map (mapper: 'Entry -> 'Mapped) (log: ILog<'Entry>) =
        log.EntryLogged
        |> Observable.map mapper
        |> create

    let internal bind (binder: 'Entry -> ITrigger<'Context>) (log: ILog<'Entry>) =
        log.EntryLogged
        |> fun obs -> obs.SelectMany(fun entry -> binder entry |> Trigger.fired)
        |> create

    let private buffer (bufferer: IObservable<'Entry> -> IObservable<#seq<'Entry>>) (log: ILog<'Entry>) =
        log.EntryLogged
        |> bufferer
        |> Observable.TimeInterval
        |> Observable.map (fun buffer -> Buffer<'Entry>(buffer.Value, buffer.Interval))
        |> create

    let bufferByInterval (interval: TimeSpan) (log: ILog<'Entry>) =
        log |> buffer (fun entries -> entries.Buffer(interval))

    let bufferByBoundaries (boundaries: IObservable<'Boundary>) (log: ILog<'Entry>) =
        log |> buffer (fun entries -> entries.Buffer(boundaries))

    let subscribe onNext (log: ILog<'Entry>) =
        log.EntryLogged |> Observable.subscribe onNext

    let subscribeForever onNext (log: ILog<'Entry>) =
        log |> subscribe onNext |> ignore

/// A log that both captures and emits entries.
type Log<'Entry>
    /// A log that both captures and emits entries.
    public () =
    // Synchronize the subject to ensure that multiple entries
    // are not logged at the same time and therefore that all
    // subscribers receive entries in the same order.
    let entryLogged = new Subject<'Entry>() |> Subject.Synchronize

    member this.LogEntry(entry) =
        entryLogged.OnNext(entry)

    member this.EntryLogged = entryLogged |> Observable.AsObservable

    interface ILog<'Entry> with
        member this.EntryLogged = this.EntryLogged
