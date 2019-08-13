using System;
using System.Diagnostics;

namespace Subliminal
{
    public class Timer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<EndedTimer> _ended;
        private bool _hasEnded;

        internal Timer()
        {
            _stopwatch = new Stopwatch();
            _ended = new Event<EndedTimer>();
            _hasEnded = false;
        }

        internal void Start()
        {
            _stopwatch.Start();
        }

        internal IEvent<EndedTimer> Ended => _ended;

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
            _ended.Raise(new EndedTimer(_stopwatch.Elapsed, wasCanceled));
            _hasEnded = true;
        }

        void IDisposable.Dispose()
        {
            Complete();
        }
    }
}
