using System;

namespace Subliminal
{
    public interface ITimer
    {
        IEventLog<TimerStarted> Started { get; }
        IEventLog<TimerEnded> Ended { get; }
        IMetric<TimeSpan> Duration { get; }
    }
}
