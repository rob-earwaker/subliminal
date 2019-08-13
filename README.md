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

## Event Log

An event log records when a particular event occurs, along with optional context information.

```csharp
using Subliminal;
using System;

// Event log without context information
var eventLog = new EventLog();

eventLog.Subscribe(_ => Console.WriteLine($"Event occurred"));

eventLog.LogOccurrence();
// "Event occurred"
eventLog.LogOccurrence();
// "Event occurred"
```

```csharp
using Subliminal;
using System;

class Context
{
    public Context(string message)
    {
        Message = message;
    }

    public string Message { get; }
}

// Event log with context information
var eventLog = new EventLog<Context>();

eventLog.Subscribe(context =>
    Console.WriteLine($"Event occurred with message '{context.Message}'"));

eventLog.LogOccurrence(new Context(message: "hello"));
// "Event occurred with message 'hello'"
eventLog.LogOccurrence(new Context(message: "world"));
// "Event occurred with message 'world'"
```

## Operation

An operation records execution timing information and is made up of several different event logs that record when an operation is started, completed, canceled or ended. An operation ends when it is either canceled or completed. Operations can also optionally include context information.

```csharp
using Subliminal;
using System;
using System.Threading;

// Operation without context information
var operation = new Operation();

operation.Started.Subscribe(started =>
    Console.WriteLine("Operation started"));

operation.Completed.Subscribe(completed =>
    Console.WriteLine($"Operation was completed after {completed.Duration}"));
    
operation.Canceled.Subscribe(canceled =>
    Console.WriteLine($"Operation was canceled after {canceled.Duration}"));
    
operation.Ended.Subscribe(ended =>
    Console.WriteLine($"Operation was ended after {ended.Duration}"));

using (var timer = operation.StartNewTimer())
{
    // "Operation started"
    Thread.Sleep(TimeSpan.FromSeconds(1));
}
// "Operation was completed after 00:00:01.0943984"
// "Operationwas ended after 00:00:01.0943984"

using (var timer = operation.StartNewTimer())
{
    // "Operation started"
    Thread.Sleep(TimeSpan.FromSeconds(1));
    timer.Complete();
    // "Operation was completed after 00:00:01.0908245"
    // "Operation was ended after 00:00:01.0908245"
    Thread.Sleep(TimeSpan.FromSeconds(1));
}

using (var timer = operation.StartNewTimer())
{
    // "Operation started"
    Thread.Sleep(TimeSpan.FromSeconds(1));
    timer.Cancel();
    // "Operation was canceled after 00:00:01.0952762"
    // "Operation was ended after 00:00:01.0952762"
    Thread.Sleep(TimeSpan.FromSeconds(1));
}
```

```csharp
using Subliminal;
using System;
using System.Threading;

class Context
{
    public Context(string message)
    {
        Message = message;
    }

    public string Message { get; }
}

// Operation with context information
var operation = new Operation<Context>();

operation.Started.Subscribe(started =>
    Console.WriteLine($"Operation started with message '{started.Context.Message}'"));

using (var timer = operation.StartNewTimer(new Context(message: "hello")))
{
    // "Operation started with message 'hello'"
    Thread.Sleep(TimeSpan.FromSeconds(1));
}

using (var timer = operation.StartNewTimer(new Context(message: "world")))
{
    // "Operation started with message 'world'"
    Thread.Sleep(TimeSpan.FromSeconds(1));
}
```
