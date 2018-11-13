module MetricTests

open FsCheck
open FsCheck.Xunit
open Lognostics
open Swensen.Unquote
open System
open System.Collections.Generic

type EventCollector<'TEventArgs when 'TEventArgs :> EventArgs>() =
    let receivedEvents = List<'TEventArgs>()

    interface IEventHandler<'TEventArgs> with
        member __.HandleEvent(sender, eventArgs) =
            receivedEvents.Add(eventArgs)

    member val ReceivedEvents = receivedEvents

let createDelegateFrom (eventHandler: IEventHandler<'TEventArgs>) =
    EventHandler<'TEventArgs>(fun sender eventArgs -> eventHandler.HandleEvent(sender, eventArgs))

[<Property>]
let ``test calls subscribed event handlers once for each value logged`` (metricValue: obj) =
    let eventHandlerCounts = Gen.choose (0, 10) |> Arb.fromGen
    Prop.forAll eventHandlerCounts (fun eventHandlerCount ->
        // Arrange
        let eventCounters = Array.init eventHandlerCount (fun _ -> EventCounter<_>())
        let metric = Metric()
        for eventCounter in eventCounters do
            metric.Sampled.AddHandler(createDelegateFrom eventCounter)
        // Act
        metric.LogValue(metricValue)
        // Assert
        for eventCounter in eventCounters do
            test <@ eventCounter.EventCount = 1 @>)

[<Property>]
let ``test includes metric value in metric sampled event args`` (metricValue: obj) =
    // Arrange
    let metric = Metric()
    let eventCollector = EventCollector()
    metric.Sampled.AddHandler(createDelegateFrom eventCollector)
    // Act
    metric.LogValue(metricValue)
    // Assert
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].Value = metricValue @>

[<Property>]
let ``test includes metric id in metric sampled event args`` (metricValue: obj) =
    // Arrange
    let metric = Metric()
    let eventCollector = EventCollector()
    metric.Sampled.AddHandler(createDelegateFrom eventCollector)
    // Act
    metric.LogValue(metricValue)
    // Assert
    test <@ eventCollector.ReceivedEvents.Count = 1 @>
    test <@ eventCollector.ReceivedEvents.[0].MetricId = metric.MetricId @>