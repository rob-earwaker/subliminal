namespace Subliminal

open System
open System.Reactive.Linq
open System.Reactive.Subjects

type internal ITrigger<'Event> =
    abstract member Fired : IObservable<'Event>

[<RequireQualifiedAccess>]
module internal Trigger =
    let private create fired =
        { new ITrigger<'Event> with
            member this.Fired = fired }

    let ofObservable (observable: IObservable<'Event>) =
        // Take a single value from the observable to ensure that the trigger
        // is only fired once, and replay this value to all observers.
        let fired = observable.Take(1) |> Observable.Replay
        // Connect immediately so that observers can consume the event.
        fired.Connect() |> ignore
        create fired

    let asTrigger (trigger: ITrigger<'Event>) =
        create trigger.Fired

    let fired (trigger: ITrigger<'Event>) =
        trigger.Fired

    let map (mapper: 'Event -> 'Mapped) (trigger: ITrigger<'Event>) =
        trigger.Fired |> Observable.map mapper |> create

    let filter predicate (trigger: ITrigger<'Event>) =
        trigger.Fired
        |> Observable.filter predicate
        |> create

    let choose (chooser: 'Event -> 'Chosen option) (trigger: ITrigger<'Event>) =
        trigger
        |> map chooser
        |> filter Option.isSome
        |> map Option.get

type internal Trigger<'Event>() =
    // Synchronize the subject to ensure that multiple events
    // are not raised at the same time and therefore that all
    // subscribers receive the same event.
    let fired = new Subject<'Event>() |> Subject.Synchronize
    let trigger = fired |> Trigger.ofObservable

    member this.Raise(event) =
        fired.OnNext(event)
        fired.OnCompleted()

    member this.Fired = trigger.Fired

    interface ITrigger<'Event> with
        member this.Fired = this.Fired
