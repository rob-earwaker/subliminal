using System;

namespace Subliminal
{
    public interface ITimer
    {
        IEventLog<TimerStarted> TimerStarted { get; }
        IEventLog<TimerEnded> TimerEnded { get; }
        IMetric<TimeSpan> Duration { get; }
    }
}
