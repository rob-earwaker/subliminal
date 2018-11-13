using System;

namespace Lognostics
{
    public class MetricSampledEventArgs<TMetric> : EventArgs
    {
        public MetricSampledEventArgs(Guid metricId, TMetric value)
        {
            MetricId = metricId;
            Value = value;
        }

        public Guid MetricId { get; }
        public TMetric Value { get; }
    }
}