# Subliminal

[![Nuget](https://img.shields.io/nuget/v/Subliminal?label=package&logo=nuget&logoColor=white)](https://www.nuget.org/packages/Subliminal/)
[![AppVeyor branch](https://img.shields.io/appveyor/ci/rob-earwaker/subliminal/master?logo=appveyor&logoColor=white)](https://ci.appveyor.com/project/rob-earwaker/subliminal/branch/master)
[![AppVeyor tests (branch)](https://img.shields.io/appveyor/tests/rob-earwaker/subliminal/master?logo=appveyor&logoColor=white&compact_message)](https://ci.appveyor.com/project/rob-earwaker/subliminal/branch/master/tests)

Subliminal lets you simply and concisely capture application metrics in library code, without having to worry about things like: where the data should be sent, what exactly gets recorded, or even whether the data is recorded at all! Metrics are exposed as streams of observable events that applications can consume with the powerful capabilities of [Reactive Extensions](https://github.com/dotnet/reactive) - unlocking operations such as transformation, sampling, aggregation and many more.

Key design principles of Subliminal:

- **Simple and concise metric capture.** Capture of metrics focuses on data collection only. The final destination of metric data, and even whether or not it gets recorded, is not specified by the metric source.
- **Always-available, not always-on.** By exposing metrics to the consumer as observable streams of data that are not recorded by default, consumers can choose only the metrics that are important to their application.
- **Configurable, not configured.** Configuration of the final destination for metric data, as well as any custom filtering or transformation, is done by the consumer not by the metric source. This gives consumers the flexibility of choosing what metric data gets sent where.

Metric types:

- `Gauge<TValue>` - A gauge is the simplest type of metric. It provides absolute values of a particular quantity, e.g. the number of messages on a queue or the current processor usage.
- `Counter<TIncrement>` - A counter provides incremental values that measure the relative changes of a particular quantity over time, e.g. the number of entities retrieved from a database or the number of bytes read from disk. Unlike a gauge, the values produced by a counter represent relative changes of the quantity not absolute values.
- `EventLog` `EventLog<TEvent>` - An event log provides notifications whenever a particular event occurs, e.g. when a message is handled or a file is deleted. Each event can optionally include context information.
- `Operation` `Operation<TContext>` - An operation provides execution timing information for a particular action, e.g. adding an item to the basket or registering a new user. It is made up of several different event logs that record when an operation is started, completed or canceled. Each operation execution can optionally include context information.
- `Log<TEntry>` - A log is a sequence of entries that provide some information, where each entry is independent of all others. In the simplest case, a log might provide debug messages in the form of text.
- `Event` `Event<TEvent>` - An event represents an action that only occurs once and will therefore only provide a single value before completing the observable sequence. All future subscribers will still receive the event value despite not having an active subscription when the event was raised.

Quickstart samples:
[`Gauge<TValue>`](Subliminal.Sample.Api/QuickstartGaugeTValue.cs)
[`Counter<TIncrement>`](Subliminal.Sample.Api/QuickstartCounterTIncrement.cs)
[`EventLog`](Subliminal.Sample.Api/QuickstartEventLog.cs)
[`EventLog<TEvent>`](Subliminal.Sample.Api/QuickstartEventLogTEvent.cs)
[`Operation`](Subliminal.Sample.Api/QuickstartOperation.cs)
[`Operation<TContext>`](Subliminal.Sample.Api/QuickstartOperationTContext.cs)
[`Log<TEntry>`](Subliminal.Sample.Api/QuickstartLogTEntry.cs)
[`Event`](Subliminal.Sample.Api/QuickstartEvent.cs)
[`Event<TEvent>`](Subliminal.Sample.Api/QuickstartEventTEvent.cs)
