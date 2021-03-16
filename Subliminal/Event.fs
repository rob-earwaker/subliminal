namespace Subliminal

open System

type IEvent =
    // TODO: should maybe use Unit here instead to allow creation form a log in C#
    inherit ILog<unit>

type IEvent<'Event> =
    inherit ILog<'Event>

[<RequireQualifiedAccess>]
module Event =
    let private create occurrences =
        { new IEvent with
            member this.Data = occurrences }

    let private create' occurrences =
        { new IEvent<'Event> with
            member this.Data = occurrences }

    let ofLog (log: ILog<unit>) =
        create log.Data

    let ofLog' (log: ILog<'Event>) =
        create' log.Data

    let asEvent (event: IEvent) =
        create event.Data

    let asEvent' (event: IEvent<'Event>) =
        create' event.Data

    let withoutContext (event: IEvent<'Event>) =
        event
        |> Log.map ignore
        |> ofLog

    let count (event: IEvent) =
        event
        |> Log.map (fun _ -> Increment(1.0))
        |> Count.ofLog
    
    let count' (event: IEvent<'Event>) =
        event
        |> Log.map (fun event -> Increment<'Event>(1.0, event))
        |> Count.ofLog'

    let rateByInterval interval event =
        event |> count |> Count.rateByInterval interval

    let rateByInterval' interval (event: IEvent<'Context>) =
        event |> count' |> Count.rateByInterval' interval

    let rateByBoundaries (boundaries: IObservable<'Boundary>) event =
        event |> count |> Count.rateByBoundaries boundaries

    let rateByBoundaries' (boundaries: IObservable<'Boundary>) (event: IEvent<'Context>) =
        event |> count' |> Count.rateByBoundaries' boundaries

type Event<'Event>() =
    let log = Log<'Event>()
    let event = Event.ofLog' log

    member this.LogOccurrence(event) =
        log.Log(event)

    member this.Data = event.Data

    interface IEvent<'Event> with
        member this.Data = this.Data

type Event() =
    let event = Event<unit>()

    member this.LogOccurrence() =
        event.LogOccurrence(())

    member this.Data = event |> Event.withoutContext |> Log.data

    interface IEvent with
        member this.Data = this.Data
