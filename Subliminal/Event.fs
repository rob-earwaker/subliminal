namespace Subliminal

open System

type IEvent =
    // TODO: should maybe use Unit here instead to allow creation form a log in C#
    abstract member Occurred : ILog<unit>

type IEvent<'Event> =
    abstract member Occurred : ILog<'Event>

[<RequireQualifiedAccess>]
module Event =
    let private create occurred =
        { new IEvent with
            member this.Occurred = occurred }

    let private create' occurred =
        { new IEvent<'Event> with
            member this.Occurred = occurred }

    let ofLog (log: ILog<unit>) =
        create log

    let ofLog' (log: ILog<'Event>) =
        create' log

    let asEvent (event: IEvent) =
        create event.Occurred

    let asEvent' (event: IEvent<'Event>) =
        create' event.Occurred

    let asLog (event: IEvent) =
        event.Occurred

    let asLog' (event: IEvent<'Event>) =
        event.Occurred

    let asObservable event =
        event |> asLog |> Log.asObservable

    let asObservable' (event: IEvent<'Event>) =
        event |> asLog' |> Log.asObservable

    let occurred (event: IEvent) =
        event.Occurred

    let occurred' (event: IEvent<'Event>) =
        event.Occurred

    let withoutContext (event: IEvent<'Event>) =
        event.Occurred
        |> Log.map ignore
        |> create

    let map' (mapper: 'Event -> 'Mapped) (event: IEvent<'Event>) =
        event.Occurred |> Log.map mapper

    let internal bind (binder: 'Event -> ITrigger<'Bound>) (event: IEvent<'Event>) =
        event.Occurred
        |> Log.bind binder
        |> ofLog'

    let count (event: IEvent) =
        event.Occurred
        |> Log.map (fun _ -> Increment(1.0))
        |> Count.ofLog
    
    let count' (event: IEvent<'Event>) =
        event.Occurred
        |> Log.map (fun event -> Increment<'Event>(1.0, event))
        |> Count.ofLog'

    let bufferByInterval' interval (event: IEvent<'Context>) =
        event.Occurred |> Log.bufferByInterval interval

    let bufferByBoundaries' (boundaries: IObservable<'Boundary>) (event: IEvent<'Context>) =
        event.Occurred |> Log.bufferByBoundaries boundaries

    let rateByInterval interval event =
        event |> count |> Count.rateByInterval interval

    let rateByInterval' interval (event: IEvent<'Context>) =
        event |> count' |> Count.rateByInterval' interval

    let rateByBoundaries (boundaries: IObservable<'Boundary>) event =
        event |> count |> Count.rateByBoundaries boundaries

    let rateByBoundaries' (boundaries: IObservable<'Boundary>) (event: IEvent<'Context>) =
        event |> count' |> Count.rateByBoundaries' boundaries

    let subscribe onNext (event: IEvent) =
        event.Occurred |> Log.subscribe onNext

    let subscribe' onNext (event: IEvent<'Event>) =
        event.Occurred |> Log.subscribe onNext

    let subscribeForever onNext (event: IEvent) =
        event.Occurred |> Log.subscribeForever onNext

    let subscribeForever' onNext (event: IEvent<'Event>) =
        event.Occurred |> Log.subscribeForever onNext

type Event<'Event>() =
    let log = Log<'Event>()
    let event = Event.ofLog' log

    member this.LogOccurrence(event) =
        log.Log(event)

    member this.Occurred = event.Occurred

    interface IEvent<'Event> with
        member this.Occurred = this.Occurred

type Event() =
    let event = Event<unit>()

    member this.LogOccurrence() =
        event.LogOccurrence(())

    member this.Occurred = event |> Event.withoutContext |> Event.occurred

    interface IEvent with
        member this.Occurred = this.Occurred
