# Subliminal

[![Build status](https://ci.appveyor.com/api/projects/status/cj2r5qjt5b88u1y2/branch/master?svg=true)](https://ci.appveyor.com/project/rob-earwaker/subliminal/branch/master)

Subliminal allows you to concisely capture custom application metrics and expose them as streams of observable events that can be consumed with the powerful capabilities of [Reactive Extensions](https://github.com/dotnet/reactive).

Key design principles of Subliminal:

- **Simple and concise metric capture.** Capture of metrics focuses on data collection only. The final destination of metric data, and even whether or not it gets recorded, is not specified by the metric source.
- **Always-available, not always-on.** By exposing metrics to the consumer as observable streams of data that are not recorded by default, consumers can choose only the metrics that are important to their application.
- **Configurable, not configured.** Configuration of the final destination for metric data and the specific values that are recorded is done by the consumer, not by the metric source. This allows consumers the flexibility of choosing what metric data gets sent where.
