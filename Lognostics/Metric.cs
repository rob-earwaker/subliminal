using Lognostics.Events;
using System;

namespace Lognostics
{
    public class Metric<TMetric>
    {
        public Metric()
        {
            MetricId = Guid.NewGuid();
        }

        public Guid MetricId { get; }

        public event EventHandler<MetricSampled<TMetric>> Sampled;

        public void LogValue(TMetric value)
        {
            Sampled?.Invoke(this, new MetricSampled<TMetric>(MetricId, value));
        }
    }
}
