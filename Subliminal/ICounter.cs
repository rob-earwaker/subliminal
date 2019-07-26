using System;

namespace Subliminal
{
    public interface ICounter
    {
        Guid CounterId { get; }
        IObservable<long> Increments { get; }
        IObservable<RateOfChange> RateOfChange { get; }
    }
}
