using System;

namespace Subliminal
{
    public class TimerEnded : ITiming
    {
        public TimerEnded(TimeSpan duration)
        {
            Duration = duration;
        }
        
        public TimeSpan Duration { get; }
    }
}
