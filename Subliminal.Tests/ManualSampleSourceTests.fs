module ManualSampleSourceTests

open Xunit
open FsCheck
open Subliminal
open Swensen.Unquote
open System.Collections.Generic

[<Fact>]
let ``test samples observed when values logged`` () =
    let runTest valueCount =
        let source = ManualSampleSource<obj>()
        let observations = Queue<Sample<obj>>()
        use subscription = source.Samples.Subscribe(observations.Enqueue)
        for _ in Array.zeroCreate valueCount do
            let value = obj()
            source.OnNext(value)
            test <@ observations.Count = 1 @>
            test <@ observations.Dequeue().Value = value @>
    let valueCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll valueCounts runTest |> Check.Quick
