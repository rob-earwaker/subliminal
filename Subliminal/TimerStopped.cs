using System;

namespace Subliminal
{
    internal sealed class TimerStopped
    {
        public TimerStopped(TimeSpan duration, bool wasCanceled)
        {
            Duration = duration;
            WasCanceled = wasCanceled;
        }

        public TimeSpan Duration { get; }
        public bool WasCanceled { get; }
    }
}
