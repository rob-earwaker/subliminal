module OperationTests

open Xunit
open FsCheck
open Subliminal
open Subliminal.Events
open Swensen.Unquote
open System.Collections.Generic

[<Fact>]
let ``test started events observed when new operation scopes started`` () =
    let runTest operationCount =
        let operation = Operation()
        let observations = Queue<OperationStarted>()
        use subscription = operation.Started.Subscribe(observations.Enqueue)
        for _ in Array.zeroCreate operationCount do
            use operationScope = operation.StartNew()
            test <@ observations.Count = 1 @>
            test <@ observations.Dequeue().Operation = operationScope @>
    let operationCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll operationCounts runTest |> Check.Quick

[<Fact>]
let ``test ended events observed when operation scopes ended`` () =
    let runTest operationCount =
        let operation = Operation()
        let observations = Queue<OperationEnded>()
        use subscription = operation.Ended.Subscribe(observations.Enqueue)
        for _ in Array.zeroCreate operationCount do
            use operationScope = operation.StartNew()
            test <@ observations.Count = 0 @>
            operationScope.End()
            test <@ observations.Count = 1 @>
            test <@ observations.Dequeue().Operation = operationScope @>
    let operationCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll operationCounts runTest |> Check.Quick

[<Fact>]
let ``test ended events observed when operation scopes disposed`` () =
    let runTest operationCount =
        let operation = Operation()
        let observations = Queue<OperationEnded>()
        use subscription = operation.Ended.Subscribe(observations.Enqueue)
        for _ in Array.zeroCreate operationCount do
            use operationScope = operation.StartNew()
            test <@ observations.Count = 0 @>
            operationScope.Dispose()
            test <@ observations.Count = 1 @>
            test <@ observations.Dequeue().Operation = operationScope @>
    let operationCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll operationCounts runTest |> Check.Quick

[<Fact>]
let ``test completed events observed when operation scopes ended`` () =
    let runTest operationCount =
        let operation = Operation()
        let observations = Queue<OperationCompleted>()
        use subscription = operation.Completed.Subscribe(observations.Enqueue)
        for _ in Array.zeroCreate operationCount do
            use operationScope = operation.StartNew()
            test <@ observations.Count = 0 @>
            operationScope.End()
            test <@ observations.Count = 1 @>
            test <@ observations.Dequeue().Operation = operationScope @>
    let operationCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll operationCounts runTest |> Check.Quick

[<Fact>]
let ``test completed events observed when operation scopes disposed`` () =
    let runTest operationCount =
        let operation = Operation()
        let observations = Queue<OperationCompleted>()
        use subscription = operation.Completed.Subscribe(observations.Enqueue)
        for _ in Array.zeroCreate operationCount do
            use operationScope = operation.StartNew()
            test <@ observations.Count = 0 @>
            operationScope.Dispose()
            test <@ observations.Count = 1 @>
            test <@ observations.Dequeue().Operation = operationScope @>
    let operationCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll operationCounts runTest |> Check.Quick

[<Fact>]
let ``test canceled events observed when operation scopes canceled`` () =
    let runTest operationCount =
        let operation = Operation()
        let observations = Queue<OperationCanceled>()
        use subscription = operation.Canceled.Subscribe(observations.Enqueue)
        for _ in Array.zeroCreate operationCount do
            use operationScope = operation.StartNew()
            test <@ observations.Count = 0 @>
            operationScope.Cancel()
            test <@ observations.Count = 1 @>
            test <@ observations.Dequeue().Operation = operationScope @>
    let operationCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll operationCounts runTest |> Check.Quick