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

        public event EventHandler<MetricSampledEventArgs<TMetric>> Sampled;

        public void LogValue(TMetric value)
        {
            Sampled?.Invoke(this, new MetricSampledEventArgs<TMetric>(MetricId, value));
        }
    }
}
