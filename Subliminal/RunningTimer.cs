using System;
using System.Diagnostics;

namespace Subliminal
{
    public class RunningTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<TimerEnded> _ended;
        private bool _hasEnded;

        public RunningTimer()
        {
            _stopwatch = Stopwatch.StartNew();
            _ended = new Event<TimerEnded>();
            _hasEnded = false;
        }
        
        public IEvent<TimerEnded> Ended => _ended;

        public void End()
        {
            if (_hasEnded)
                return;

            _stopwatch.Stop();

            _ended.Log(new TimerEnded(_stopwatch.Elapsed));

            _hasEnded = true;
        }

        public void Dispose()
        {
            End();
        }
    }
}
