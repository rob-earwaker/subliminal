using System;

namespace Subliminal
{
    public class TimerStarted
    {
        public TimerStarted(Guid timerId, IEvent<TimerEnded> endedEvent)
        {
            TimerId = timerId;
            EndedEvent = endedEvent;
        }

        public Guid TimerId { get; }
        public IEvent<TimerEnded> EndedEvent { get; }
    }
}
