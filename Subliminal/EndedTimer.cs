using System;

namespace Subliminal
{
    internal class EndedTimer
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
