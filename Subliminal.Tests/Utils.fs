namespace Subliminal.Tests

open System

type TestObserver<'Value>() =
    member val ObservedValues = [] with get, set
    member val ObservableCompleted = false with get, set

    interface IObserver<'Value> with
        member this.OnNext(value) =
            this.ObservedValues <- List.append this.ObservedValues [ value ]

        member this.OnCompleted() =
            this.ObservableCompleted <- true
        
        member this.OnError(error) =
            raise <| exn("Observable raised an error", error)
            
type Wrapper<'TValue> = { Value: 'TValue }
