using System;

namespace Lognostics
{
    public class Metric<TMetric>
    {
        public event EventHandler<MetricSampledEventArgs<TMetric>> Sampled;

        public void LogValue(TMetric value)
        {
            Sampled?.Invoke(this, new MetricSampledEventArgs<TMetric>(value));
        }
    }
}
