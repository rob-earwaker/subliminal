namespace Subliminal

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
