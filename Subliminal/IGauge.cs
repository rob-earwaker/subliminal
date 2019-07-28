using System;

namespace Subliminal
{
    public interface IGauge<TValue>
    {
        Guid GaugeId { get; }
        IObservable<GaugeSample<TValue>> Sampled { get; }
    }
}
