using System;

namespace Lognostics
{
    public class MetricSampledEventArgs<TMetric> : EventArgs
    {
        public MetricSampledEventArgs(TMetric value)
        {
            Value = value;
        }

        public TMetric Value { get; }
    }
}