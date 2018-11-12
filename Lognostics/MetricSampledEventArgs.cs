using System;

namespace Lognostics
{
    public class MetricSampledEventArgs<TValue> : EventArgs
    {
        public MetricSampledEventArgs(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }
    }
}