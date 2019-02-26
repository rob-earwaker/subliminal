module ManualGaugeTests

open Xunit
open FsCheck
open Subliminal
open Swensen.Unquote
open System.Collections.Generic

[<Fact>]
let ``test samples observed when values logged`` () =
    let runTest valueCount =
        let gauge = ManualGauge<obj>()
        let observations = Queue<Sample<obj>>()
        use subscription = gauge.Sampled.Subscribe(observations.Enqueue)
        for _ in Array.zeroCreate valueCount do
            let value = obj()
            gauge.LogValue(value)
            test <@ observations.Count = 1 @>
            test <@ observations.Dequeue().Value = value @>
    let valueCounts = Gen.choose (0, 100) |> Arb.fromGen
    Prop.forAll valueCounts runTest |> Check.Quick
