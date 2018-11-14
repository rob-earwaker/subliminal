module ScopeTests

open Xunit
open FsCheck
open FsCheck.Xunit
open Lognostics
open Swensen.Unquote
open System
open Utils

type Arbitrary =
    static member Scope () =
        Gen.elements (seq {
            yield new Scope() :> IScope
            yield new OperationScope(Guid.NewGuid()) :> IScope })
        |> Arb.fromGen

[<Fact>]
let ``test scope has started after start new`` () =
    use scope = Scope.StartNew()
    test <@ scope.HasStarted @>

[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scopes have different scope ids`` (scope1: IScope) (scope2: IScope) =
    test <@ scope1.ScopeId <> scope2.ScopeId @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope duration is zero if not started`` (scope: IScope) =
    test <@ scope.Duration = TimeSpan.Zero @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope is not started by default`` (scope: IScope) =
    test <@ not scope.HasStarted @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope has not ended by default`` (scope: IScope) =
    test <@ not scope.HasEnded @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope is started after explicit start`` (scope: IScope) =
    scope.Start()
    test <@ scope.HasStarted @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test start can be called multiple times`` (scope: IScope) =
    scope.Start()
    scope.Start()
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope has ended after ending`` (scope: IScope) =
    scope.Start()
    scope.End()
    test <@ scope.HasEnded @>
    
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
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event raised when scope ended`` (scope: IScope) =
    scope.Start()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.End()
    test <@ eventCounter.EventCount = 1 @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event raised when scope disposed`` (scope: IScope) =
    scope.Start()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.Dispose()
    test <@ eventCounter.EventCount = 1 @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event args contains scope`` (scope: IScope) =   
    scope.Start()
    let eventCollector = EventCollector()
    scope.Ended.AddHandler(createDelegateFrom eventCollector)
    scope.End()
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].Scope = scope @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event only raised first time scope ended`` (scope: IScope) =
    scope.Start()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.End()
    test <@ eventCounter.EventCount = 1 @>
    scope.End()
    test <@ eventCounter.EventCount = 1 @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test scope ended event not raised if scope not started`` (scope: IScope) =
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.End()
    test <@ eventCounter.EventCount = 0 @>
    
[<Property(Arbitrary=[| typeof<Arbitrary> |])>]
let ``test additional scope ended event not raised if scope ended after starting again`` (scope: IScope) =
    scope.Start()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.End()
    test <@ eventCounter.EventCount = 1 @>
    scope.Start()
    scope.End()
    test <@ eventCounter.EventCount = 1 @>
