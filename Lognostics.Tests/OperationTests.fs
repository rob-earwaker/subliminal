module OperationTests

open Lognostics
open Swensen.Unquote
open Utils
open Xunit

[<Fact>]
let ``test operations have unique operation type ids`` () =
    let operation1 = Operation()
    let operation2 = Operation()
    test <@ operation1.OperationTypeId <> operation2.OperationTypeId @>

[<Fact>]
let ``test new operation scope has correct operation type id`` () =
    let operation = Operation()
    let eventCollector = EventCollector()
    operation.Started.AddHandler(createDelegateFrom eventCollector)
    use operationScope = operation.StartNew()
    test <@ operationScope.OperationTypeId = operation.OperationTypeId @>

[<Fact>]
let ``test no active scopes when no operations started`` () =
    let operation = Operation()
    test <@ operation.ActiveScopes.Count = 0 @>

[<Fact>]
let ``test operation started event raised when new operation scope started`` () =
    let operation = Operation()
    let eventCounter = EventCounter()
    operation.Started.AddHandler(createDelegateFrom eventCounter)
    test <@ eventCounter.EventCount = 0 @>
    use operationScope= operation.StartNew()
    test <@ eventCounter.EventCount = 1 @>

[<Fact>]
let ``test operation completed event raised when operation scope ended`` () =
    let operation = Operation()
    let eventCounter = EventCounter()
    operation.Completed.AddHandler(createDelegateFrom eventCounter)
    use operationScope = operation.StartNew()
    test <@ eventCounter.EventCount = 0 @>
    operationScope.End()
    test <@ eventCounter.EventCount = 1 @>

[<Fact>]
let ``test operation completed event raised when operation scope disposed`` () =
    let operation = Operation()
    let eventCounter = EventCounter()
    operation.Completed.AddHandler(createDelegateFrom eventCounter)
    use operationScope = operation.StartNew()
    test <@ eventCounter.EventCount = 0 @>
    operationScope.Dispose()
    test <@ eventCounter.EventCount = 1 @>

[<Fact>]
let ``test includes operation scope in operation started event args`` () =
    let operation = Operation()
    let eventCollector = EventCollector()
    operation.Started.AddHandler(createDelegateFrom eventCollector)
    use operationScope = operation.StartNew()
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].OperationScope = operationScope @>

[<Fact>]
let ``test includes operation scope in operation completed event args`` () =
    let operation = Operation()
    let eventCollector = EventCollector()
    operation.Completed.AddHandler(createDelegateFrom eventCollector)
    use operationScope = operation.StartNew()
    operationScope.End()
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].OperationScope = operationScope @>