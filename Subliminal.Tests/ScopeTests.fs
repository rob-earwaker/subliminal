module ScopeTests

open Xunit
open FsCheck
open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System
open System.Collections.Generic
open System.Reactive

type Arbitrary =
    static member Scope () =
        Gen.elements (seq {
            yield new Scope() :> IScope
            yield new OperationScope() :> IScope })
        |> Arb.fromGen

[<Fact>]
let ``test scope has started after start new`` () =
    use scope = Scope.StartNew()
    test <@ scope.HasStarted @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope duration is zero if not started`` (scope: IScope) =
    test <@ scope.Duration = TimeSpan.Zero @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope is not started by default`` (scope: IScope) =
    test <@ not scope.HasStarted @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope has not ended by default`` (scope: IScope) =
    test <@ not scope.HasEnded @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope is started after explicit start`` (scope: IScope) =
    scope.Start()
    test <@ scope.HasStarted @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test start can be called multiple times`` (scope: IScope) =
    scope.Start()
    scope.Start()
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope has ended after ending`` (scope: IScope) =
    scope.Start()
    scope.End()
    test <@ scope.HasEnded @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope has ended after disposing`` (scope: IScope) =
    scope.Start()
    scope.Dispose()
    test <@ scope.HasEnded @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope duration does not increase if started after ending`` (scope: IScope) =
    scope.Start()
    scope.End()
    let duration = scope.Duration
    scope.Start()
    scope.End()
    test <@ scope.Duration = duration @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event observed when scope ended`` (scope: IScope) =
    scope.Start()
    let observations = Queue<Unit>()
    use subscription = scope.Ended.Subscribe(observations.Enqueue)
    scope.End()
    test <@ observations.Count = 1 @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event raised when scope disposed`` (scope: IScope) =
    scope.Start()
    let observations = Queue<Unit>()
    use subscription = scope.Ended.Subscribe(observations.Enqueue)
    scope.Dispose()
    test <@ observations.Count = 1 @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event only raised first time scope ended`` (scope: IScope) =
    scope.Start()
    let observations = Queue<Unit>()
    use subscription = scope.Ended.Subscribe(observations.Enqueue)
    scope.End()
    test <@ observations.Count = 1 @>
    scope.End()
    test <@ observations.Count = 1 @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event not raised if scope not started`` (scope: IScope) =
    let observations = Queue<Unit>()
    use subscription = scope.Ended.Subscribe(observations.Enqueue)
    scope.End()
    test <@ observations.Count = 0 @>
    scope.Dispose()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test additional scope ended event not raised if scope ended after starting again`` (scope: IScope) =
    scope.Start()
    let observations = Queue<Unit>()
    use subscription = scope.Ended.Subscribe(observations.Enqueue)
    scope.End()
    test <@ observations.Count = 1 @>
    scope.Start()
    scope.End()
    test <@ observations.Count = 1 @>
    scope.Dispose()
