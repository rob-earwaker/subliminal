using System;
using System.Diagnostics;

namespace Subliminal
{
    public class RunningTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly TimingEvent<TimerEnded> _ended;
        private bool _hasEnded;

        public RunningTimer()
        {
            _stopwatch = Stopwatch.StartNew();
            _ended = new TimingEvent<TimerEnded>();
            _hasEnded = false;
        }
        
        public ITimingEvent<TimerEnded> Ended => _ended;

        public void End()
        {
            if (_hasEnded)
                return;

            _stopwatch.Stop();

            _ended.LogAndClose(new TimerEnded(_stopwatch.Elapsed));

            _hasEnded = true;
        }

        public void Dispose()
        {
            End();
        }
    }
}
