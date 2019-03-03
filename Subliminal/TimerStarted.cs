using System;

namespace Subliminal
{
    public class TimerStarted
    {
        public TimerStarted(Guid timerId, IEvent<TimerEnded> ended)
        {
            TimerId = timerId;
            Ended = ended;
        }

        public Guid TimerId { get; }
        public IEvent<TimerEnded> Ended { get; }
    }
}
