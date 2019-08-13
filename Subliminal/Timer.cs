using System;
using System.Diagnostics;

namespace Subliminal
{
    public class Timer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<TimerEnded> _ended;
        private bool _hasEnded;

        internal Timer()
        {
            _stopwatch = new Stopwatch();
            _ended = new Event<TimerEnded>();
            _hasEnded = false;
        }

        internal void Start()
        {
            _stopwatch.Start();
        }

        internal IEvent<TimerEnded> Ended => _ended;

        public void Complete()
        {
            End(wasCanceled: false);
        }

        public void Cancel()
        {
            End(wasCanceled: true);
        }

        private void End(bool wasCanceled)
        {
            if (_hasEnded)
                return;

            _stopwatch.Stop();
            _ended.Raise(new TimerEnded(_stopwatch.Elapsed, wasCanceled));
            _hasEnded = true;
        }

        void IDisposable.Dispose()
        {
            Complete();
        }
    }
}
