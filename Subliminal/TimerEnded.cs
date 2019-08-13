using System;

namespace Subliminal
{
    internal class TimerEnded
    {
        public TimerEnded(TimeSpan duration, bool wasCanceled)
        {
            Duration = duration;
            WasCanceled = wasCanceled;
        }

        public TimeSpan Duration { get; }
        public bool WasCanceled { get; }
    }
}
