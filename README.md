# Subliminal

[![Build status](https://ci.appveyor.com/api/projects/status/cj2r5qjt5b88u1y2/branch/master?svg=true)](https://ci.appveyor.com/project/rob-earwaker/subliminal/branch/master)

Subliminal allows you to simply and concisely capture custom application metrics and expose them as streams of observable events that can be consumed with the powerful capabilities of [Reactive Extensions](https://github.com/dotnet/reactive).

Key design principles of Subliminal:

- **Simple and concise metric capture.** Capture of metrics focuses on data collection only. The final destination of metric data, and even whether or not it gets recorded, is not specified by the metric source.
- **Always-available, not always-on.** By exposing metrics to the consumer as observable streams of data that are not recorded by default, consumers can choose only the metrics that are important to their application.
- **Configurable, not configured.** Configuration of the final destination for metric data and the specific values that are recorded is done by the consumer, not by the metric source. This gives consumers the flexibility of choosing what metric data gets sent where.

## Gauge

A gauge is the simplest type of metric. It provides absolute values of a particular quantity, e.g. the number of messages on a queue or the current processor usage.

```csharp
using Subliminal;
using System;

var gauge = new Gauge<double>();

gauge.Subscribe(value =>
    Console.WriteLine($"Current gauge value is {value}"));

gauge.LogValue(2.34);
// "Current gauge value is 2.34"
gauge.LogValue(59.41);
// "Current gauge value is 59.41"
```

## Counter

A counter provides incremental values that measure the relative changes of a quantity over time, which can be as simple as measuring the number of occurrences of a particular event. Unlike a gauge, the values produced by a counter represent relative changes in the quantity not absolute values.

```csharp
using Subliminal;
using System;

var counter = new Counter<long>();

counter.Subscribe(increment =>
    Console.WriteLine($"Counter incremented by {increment}"));

counter.IncrementBy(2);
// "Counter incremented by 2"
counter.IncrementBy(125);
// "Counter incremented by 125"
```
