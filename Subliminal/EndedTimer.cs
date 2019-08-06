using System;

namespace Subliminal
{
    public class EndedTimer
    {
        public EndedTimer(TimeSpan duration, bool wasCanceled)
        {
            Duration = duration;
            WasCanceled = wasCanceled;
        }

        public TimeSpan Duration { get; }
        public bool WasCanceled { get; }
    }
}
