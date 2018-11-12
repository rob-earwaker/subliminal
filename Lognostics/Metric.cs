using System;

namespace Lognostics
{
    public class Metric<TValue>
    {
        public event EventHandler<MetricSampledEventArgs<TValue>> Sampled;

        public void LogValue(TValue value)
        {
            Sampled?.Invoke(this, new MetricSampledEventArgs<TValue>(value));
        }
    }
}
