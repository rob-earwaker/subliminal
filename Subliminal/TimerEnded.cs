using System;

namespace Subliminal
{
    public class TimerEnded
    {
        public TimerEnded(Guid timerId, TimeSpan duration)
        {
            TimerId = timerId;
            Duration = duration;
        }

        public Guid TimerId { get; }
        public TimeSpan Duration { get; }
    }
}
