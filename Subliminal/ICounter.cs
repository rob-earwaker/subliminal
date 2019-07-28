using System;

namespace Subliminal
{
    public interface ICounter
    {
        Guid CounterId { get; }
        IObservable<CounterIncrement> Incremented { get; }
    }
}
