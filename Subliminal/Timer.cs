using System;
using System.Diagnostics;

namespace Subliminal
{
    public sealed class Timer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<TimerStopped> _stopped;

        internal Timer()
        {
            _stopwatch = new Stopwatch();
            _stopped = new Event<TimerStopped>();
        }

        internal void Start()
        {
            _stopwatch.Start();
        }

        internal IEvent<TimerStopped> Stopped => _stopped;

        public void Stop()
        {
            Stop(wasCanceled: false);
        }

        public void Cancel()
        {
            Stop(wasCanceled: true);
        }

        private void Stop(bool wasCanceled)
        {
            if (!_stopwatch.IsRunning)
                return;

            _stopwatch.Stop();
            _stopped.Raise(new TimerStopped(_stopwatch.Elapsed, wasCanceled));
        }

        void IDisposable.Dispose()
        {
            Stop();
        }
    }
}
