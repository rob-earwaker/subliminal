using System;

namespace Subliminal
{
    public class TimerEnded
    {
        public TimerEnded(TimeSpan duration)
        {
            Duration = duration;
        }
        
        public TimeSpan Duration { get; }
    }
}
