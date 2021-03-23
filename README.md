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

- **Simple and concise data capture.** Metric capture focuses on data collection only. The final destination of metric data, and even whether or not it gets recorded, is not specified by the data source.
- **Always-available, not always-on.** By exposing data to the consumer as observable streams that are not recorded by default, consumers can choose the data that's important to their application and transform it to fit their use case.
- **Configurable, not configured.** Configuration of the final destination for data, as well as any custom filtering, transformation or aggregation, is done by the consumer not by the data source. This gives consumers the flexibility of choosing what data gets sent where, and in what shape.

## Data Collectors

| Log Type | Description |
|-|-|
| `Log<'Data>` | A log is the most fundamental way of exposing useful information as a stream of data. In the simplest case, a log might provide debug messages in the form of text. In a more complex case, a log might provide a set of related metrics that change over time, e.g. system resource usage. |
| `Gauge` | A gauge is a log of floating point values that represent changes in a single metric over time, e.g. the number of messages on a queue or the current processor usage. |
| `Gauge<'Context>` | A gauge that has some contextual information associated with it, e.g. if your gauge was capturing the number of open connections, you might want to include the name of the host as context. |
| `Counter` | A counter is a log of floating point values that represent independent measures of the same logical metric, e.g. the number of entities retrieved from a database or the number of bytes written to disk. |
| `Counter<'Context>` | A counter that has some contextual data associated with it, e.g. if you were capturing the size of messages being sent to a message broker you might want to include a message type name or the broker name as context. |
| `Event` | An event is a log that doesn't have any data associated with it but records occurrences of particular event, e.g. when a message is handled or when a file is deleted. |
| `Event<'Event>` | An event that has some data associated with it, e.g. if you were capturing occurrences of failed requests you might want to include the failure reason or failure code as event data. |
| `Operation` | An operation provides execution timing information for a particular action, e.g. adding an item to a basket or registering a new user. Data is exposed in the form of several different events that are emitted when an operation is started, stopped, completed or canceled. |
| `Operation<'Context>` | An operation that has some contextual information associated with it, e.g. if your operation was capturing the time taken to execute database queries you might want to include the database name or a query identifier as context. |
