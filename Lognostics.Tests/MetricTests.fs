module MetricTests

open FsCheck.Xunit
open Lognostics
open Swensen.Unquote
open Utils
open Xunit

[<Fact>]
let ``test metrics have unique metric ids`` () =
    let metric1 = Metric()
    let metric2 = Metric()
    test <@ metric1.MetricId <> metric2.MetricId @>

[<Property>]
let ``test includes metric value in metric sampled event args`` (metricValue: obj) =
    let metric = Metric()
    let eventCollector = EventCollector()
    metric.Sampled.AddHandler(createDelegateFrom eventCollector)
    metric.LogValue(metricValue)
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].Value = metricValue @>

[<Property>]
let ``test includes metric id in metric sampled event args`` (metricValue: obj) =
    let metric = Metric()
    let eventCollector = EventCollector()
    metric.Sampled.AddHandler(createDelegateFrom eventCollector)
    metric.LogValue(metricValue)
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].MetricId = metric.MetricId @>