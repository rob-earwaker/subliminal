namespace Subliminal.Tests

open System
open System.Collections.Generic

type TestObserver<'Value>() =
    member val ObservedValues = List<'Value>()
    member val ObservableCompleted = false with get, set

    interface IObserver<'Value> with
        member this.OnNext(value) =
            this.ObservedValues.Add(value)

        member this.OnCompleted() =
            this.ObservableCompleted <- true
        
        member this.OnError(error) =
            raise <| exn("Observable raised an error", error)