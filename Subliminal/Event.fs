namespace Subliminal

open System.Reactive

type IEvent =
    inherit ILog<Unit>

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

    let ofLog (log: ILog<Unit>) =
        create log.Data

    let ofLog' (log: ILog<'Event>) =
        create' log.Data

    let asEvent (event: IEvent) =
        create event.Data

    let asEvent' (event: IEvent<'Event>) =
        create' event.Data

    let withoutContext (event: IEvent<'Event>) =
        event
        |> Log.map (fun _ -> Unit.Default)
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
    let event = Event<Unit>()

    member this.LogOccurrence() =
        event.LogOccurrence(Unit.Default)

    member this.Data = event.Data

    interface IEvent with
        member this.Data = this.Data
