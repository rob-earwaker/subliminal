module ScopeTests

open Lognostics
open Swensen.Unquote
open System
open Utils
open Xunit

[<Fact>]
let ``test scopes have different scope ids`` () =
    use scope1 = Scope.StartNew()
    use scope2 = Scope.StartNew()
    test <@ scope1.ScopeId <> scope2.ScopeId @>
    
[<Fact>]
let ``test scope duration is zero if not started`` () =
    use scope = new Scope()
    test <@ scope.Duration = TimeSpan.Zero @>
    
[<Fact>]
let ``test scope is not started by default`` () =
    use scope = new Scope()
    test <@ not scope.HasStarted @>
    
[<Fact>]
let ``test scope has not ended by default`` () =
    use scope = new Scope()
    test <@ not scope.HasEnded @>
    
[<Fact>]
let ``test scope is started after start new`` () =
    use scope = Scope.StartNew()
    test <@ scope.HasStarted @>
    
[<Fact>]
let ``test scope is started after explicit start`` () =
    use scope = new Scope()
    scope.Start()
    test <@ scope.HasStarted @>
    
[<Fact>]
let ``test start can be called multiple times`` () =
    use scope = Scope.StartNew()
    scope.Start()
    scope.Start()
    
[<Fact>]
let ``test scope has ended after ending`` () =
    use scope = Scope.StartNew()
    scope.End()
    test <@ scope.HasEnded @>
    
[<Fact>]
let ``test scope has ended after disposing`` () =
    use scope = Scope.StartNew()
    scope.Dispose()
    test <@ scope.HasEnded @>
    
[<Fact>]
let ``test scope duration does not increase if started after ending`` () =
    use scope = Scope.StartNew()
    scope.End()
    let duration = scope.Duration
    scope.Start()
    scope.End()
    test <@ scope.Duration = duration @>

[<Fact>]
let ``test scope ended event raised when scope ended`` () =
    use scope = Scope.StartNew()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.End()
    test <@ eventCounter.EventCount = 1 @>

[<Fact>]
let ``test scope ended event raised when scope disposed`` () =
    use scope = Scope.StartNew()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.Dispose()
    test <@ eventCounter.EventCount = 1 @>

[<Fact>]
let ``test scope ended event args contains scope`` () =   
    use scope = Scope.StartNew()
    let eventCollector = EventCollector()
    scope.Ended.AddHandler(createDelegateFrom eventCollector)
    scope.End()
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].Scope = (scope :> IScope) @>

[<Fact>]
let ``test scope ended event only raised first time scope ended`` () =
    use scope = Scope.StartNew()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.End()
    test <@ eventCounter.EventCount = 1 @>
    scope.End()
    test <@ eventCounter.EventCount = 1 @>

[<Fact>]
let ``test scope ended event not raised if scope not started`` () =
    use scope = new Scope()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.End()
    test <@ eventCounter.EventCount = 0 @>

[<Fact>]
let ``test additional scope ended event not raised if scope ended after starting again`` () =
    use scope = Scope.StartNew()
    let eventCounter = EventCounter()
    scope.Ended.AddHandler(createDelegateFrom eventCounter)
    scope.End()
    test <@ eventCounter.EventCount = 1 @>
    scope.Start()
    scope.End()
    test <@ eventCounter.EventCount = 1 @>
