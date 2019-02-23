module OperationScopeTests

open Subliminal
open Subliminal.Events
open Swensen.Unquote
open System
open System.Collections.Generic
open System.Reactive
open Xunit

[<Fact>]
let ``test has started after start new`` () =
    use operation = OperationScope.StartNew()
    test <@ operation.HasStarted @>
    
[<Fact>]
let ``test duration is zero if not started`` () =
    use operation = new OperationScope()
    test <@ operation.Duration = TimeSpan.Zero @>
    
[<Fact>]
let ``test not started by default`` () =
    use operation = new OperationScope()
    test <@ not operation.HasStarted @>
    
[<Fact>]
let ``test not ended by default`` () =
    use operation = new OperationScope()
    test <@ not operation.HasEnded @>
    
[<Fact>]
let ``test started after explicit start`` () =
    use operation = new OperationScope()
    operation.Start()
    test <@ operation.HasStarted @>
    
[<Fact>]
let ``test start can be called multiple times`` () =
    use operation = new OperationScope()
    operation.Start()
    operation.Start()
    
[<Fact>]
let ``test has ended after ending`` () =
    use operation = new OperationScope()
    operation.Start()
    operation.End()
    test <@ operation.HasEnded @>
    
[<Fact>]
let ``test has ended after disposing`` () =
    use operation = new OperationScope()
    operation.Start()
    operation.Dispose()
    test <@ operation.HasEnded @>
    
[<Fact>]
let ``test duration does not increase if started after ending`` () =
    use operation = new OperationScope()
    operation.Start()
    operation.End()
    let duration = operation.Duration
    operation.Start()
    operation.End()
    test <@ operation.Duration = duration @>
    
[<Fact>]
let ``test ended event observed when ended`` () =
    use operation = new OperationScope()
    operation.Start()
    let observations = Queue<OperationEnded>()
    use subscription = operation.Ended.Subscribe(observations.Enqueue)
    operation.End()
    test <@ observations.Count = 1 @>
    
[<Fact>]
let ``test ended event observed when disposed`` () =
    use operation = new OperationScope()
    operation.Start()
    let observations = Queue<OperationEnded>()
    use subscription = operation.Ended.Subscribe(observations.Enqueue)
    operation.Dispose()
    test <@ observations.Count = 1 @>
    
[<Fact>]
let ``test ended event only observed first time ended`` () =
    use operation = new OperationScope()
    operation.Start()
    let observations = Queue<OperationEnded>()
    use subscription = operation.Ended.Subscribe(observations.Enqueue)
    operation.End()
    test <@ observations.Count = 1 @>
    operation.End()
    test <@ observations.Count = 1 @>
    
[<Fact>]
let ``test ended event not observed if not started`` () =
    use operation = new OperationScope()
    let observations = Queue<OperationEnded>()
    use subscription = operation.Ended.Subscribe(observations.Enqueue)
    operation.End()
    test <@ observations.Count = 0 @>
    
[<Fact>]
let ``test additional ended event not observed if ended after starting again`` () =
    use operation = new OperationScope()
    operation.Start()
    let observations = Queue<OperationEnded>()
    use subscription = operation.Ended.Subscribe(observations.Enqueue)
    operation.End()
    test <@ observations.Count = 1 @>
    operation.Start()
    operation.End()
    test <@ observations.Count = 1 @>
