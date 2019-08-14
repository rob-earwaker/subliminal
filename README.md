# Subliminal

[![Build status](https://ci.appveyor.com/api/projects/status/cj2r5qjt5b88u1y2/branch/master?svg=true)](https://ci.appveyor.com/project/rob-earwaker/subliminal/branch/master)

Subliminal allows you to simply and concisely capture custom application metrics and expose them as streams of observable events that can be consumed with the powerful capabilities of [Reactive Extensions](https://github.com/dotnet/reactive).

Key design principles of Subliminal:

- **Simple and concise metric capture.** Capture of metrics focuses on data collection only. The final destination of metric data, and even whether or not it gets recorded, is not specified by the metric source.
- **Always-available, not always-on.** By exposing metrics to the consumer as observable streams of data that are not recorded by default, consumers can choose only the metrics that are important to their application.
- **Configurable, not configured.** Configuration of the final destination for metric data and the specific values that are recorded is done by the consumer, not by the metric source. This gives consumers the flexibility of choosing what metric data gets sent where.

Metric types:

- `Gauge<TValue>` - A gauge is the simplest type of metric. It provides absolute values of a particular quantity, e.g. the number of messages on a queue or the current processor usage.
- `Counter<TIncrement>` - A counter provides incremental values that measure the relative changes of a particular quantity over time, e.g. the number of entities retrieved from a database or the number of bytes read from disk. Unlike a gauge, the values produced by a counter represent relative changes of the quantity not absolute values.
- `EventLog` `EventLog<TEvent>` - An event log provides notifications whenever a particular event occurs, e.g. when a message is handled or a file is deleted. Each event can optionally include context information.
- `Operation` `Operation<TContext>` - An operation provides execution timing information for a particular action, e.g. adding an item to the basket or registering a new user. It is made up of several different event logs that record when an operation is started, completed, canceled or ended. An operation ends when it is either canceled or completed. Each operation execution can optionally include context information.

Quickstart samples:
[`Gauge<TValue>`](Subliminal.Sample.Api/QuickstartGaugeTValue.cs)
[`Counter<TIncrement>`](Subliminal.Sample.Api/QuickstartCounterTIncrement.cs)
[`EventLog`](Subliminal.Sample.Api/QuickstartEventLog.cs)
[`EventLog<TEvent>`](Subliminal.Sample.Api/QuickstartEventLogTEvent.cs)
[`Operation`](Subliminal.Sample.Api/QuickstartOperation.cs)
[`Operation<TContext>`](Subliminal.Sample.Api/QuickstartOperationTContext.cs)
