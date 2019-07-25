using System;

namespace Subliminal
{
    public interface IMetric<TValue>
    {
        Guid MetricId { get; }
        IObservable<TValue> Values { get; }
    }
}
