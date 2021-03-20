# Subliminal

[![Nuget](https://img.shields.io/nuget/v/Subliminal?label=package&logo=nuget&logoColor=white)](https://www.nuget.org/packages/Subliminal/)
[![AppVeyor branch](https://img.shields.io/appveyor/ci/rob-earwaker/subliminal/master?logo=appveyor&logoColor=white)](https://ci.appveyor.com/project/rob-earwaker/subliminal/branch/master)
[![AppVeyor tests (branch)](https://img.shields.io/appveyor/tests/rob-earwaker/subliminal/master?logo=appveyor&logoColor=white&compact_message)](https://ci.appveyor.com/project/rob-earwaker/subliminal/branch/master/tests)

Subliminal lets you instrument your library code in a simple and concise way, letting you focus on capturing data and not on the exact way in which the data will be consumed. Captured data is exposed as a stream that can be transformed, filtered, aggregated and subscribed to by consuming applications, and then sent to whichever monitoring solution the application happens to be using.

```fsharp
// Library

module Processor =
    // Create a new log that captures information about an operation.
    let Operation = Operation()

    let processData data =
        // Start a new operation timer.
        use timer = Operation.StartNew()
        // Process the data.
        // ...
```

```fsharp
// Application

// Subscribe to all completed operations.
Processor.Operation.Completed
|> Log.subscribeForever (fun completed ->
    // Send information to the console.
    printfn "Processing operation took %.4f seconds to complete"
        completed.OperationId completed.Duration.TotalSeconds)

// Do some data processing.
for data in dataSet do
    Processor.processData data

// Output:
// > Processing operation took 0.1204 seconds to complete
// > Processing operation took 0.9371 seconds to complete
// > Processing operation took 0.5710 seconds to complete
// > ...
```

Key design principles of Subliminal:

- **Simple and concise metric capture.** Capture of metrics focuses on data collection only. The final destination of metric data, and even whether or not it gets recorded, is not specified by the metric source.
- **Always-available, not always-on.** By exposing metrics to the consumer as observable streams of data that are not recorded by default, consumers can choose only the metrics that are important to their application.
- **Configurable, not configured.** Configuration of the final destination for metric data, as well as any custom filtering or transformation, is done by the consumer not by the metric source. This gives consumers the flexibility of choosing what metric data gets sent where.

Metric types:

- `Gauge` - A gauge is the simplest type of metric. It provides absolute values of a particular quantity, e.g. the number of messages on a queue or the current processor usage.
- `Counter` - A counter provides incremental values that measure the relative changes of a particular quantity over time, e.g. the number of entities retrieved from a database or the number of bytes read from disk. Unlike a gauge, the values produced by a counter represent relative changes of the quantity not absolute values.
- `EventLog` `EventLog<TEvent>` - An event log provides notifications whenever a particular event occurs, e.g. when a message is handled or a file is deleted. Each event can optionally include context information.
- `OperationLog` `OperationLog<TContext>` - An operation provides execution timing information for a particular action, e.g. adding an item to the basket or registering a new user. It is made up of several different event logs that record when an operation is started, completed or canceled. Each operation execution can optionally include context information.
- `Log<TEntry>` - A log is a sequence of entries that provide some information, where each entry is independent of all others. In the simplest case, a log might provide debug messages in the form of text.
- `Event` `Event<TEvent>` - An event represents an action that only occurs once and will therefore only provide a single value before completing the observable sequence. All future subscribers will still receive the event value despite not having an active subscription when the event was raised.

Quickstart samples:
[`Gauge`](Subliminal.Sample.Api/QuickstartGauge.cs)
[`Counter`](Subliminal.Sample.Api/QuickstartCounter.cs)
[`EventLog`](Subliminal.Sample.Api/QuickstartEventLog.cs)
[`EventLog<TEvent>`](Subliminal.Sample.Api/QuickstartEventLogTEvent.cs)
[`OperationLog`](Subliminal.Sample.Api/QuickstartOperationLog.cs)
[`OperationLog<TContext>`](Subliminal.Sample.Api/QuickstartOperationLogTContext.cs)
[`Log<TEntry>`](Subliminal.Sample.Api/QuickstartLogTEntry.cs)
[`Event`](Subliminal.Sample.Api/QuickstartEvent.cs)
[`Event<TEvent>`](Subliminal.Sample.Api/QuickstartEventTEvent.cs)
