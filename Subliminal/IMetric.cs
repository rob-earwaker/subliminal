using System;

namespace Subliminal
{
    public interface IMetric<TValue> : IObservable<TValue>
    {
        Guid MetricId { get; }
    }
}
