using System;

namespace Subliminal
{
    public interface ITimer
    {
        Guid TimerId { get; }
        IObservable<TimeSpan> Durations { get; }
    }
}
