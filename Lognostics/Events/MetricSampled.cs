using System;

namespace Lognostics.Events
{
    public class MetricSampled<TMetric> : EventArgs
    {
        public MetricSampled(Guid metricId, TMetric value)
        {
            MetricId = metricId;
            Value = value;
        }

        public Guid MetricId { get; }
        public TMetric Value { get; }
    }
}