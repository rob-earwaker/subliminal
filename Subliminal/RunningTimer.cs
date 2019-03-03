using System;
using System.Diagnostics;

namespace Subliminal
{
    public class RunningTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<TimerEnded> _endedEvent;
        private bool _hasEnded;

        public RunningTimer()
        {
            _stopwatch = Stopwatch.StartNew();
            _endedEvent = new Event<TimerEnded>();
            _hasEnded = false;

            TimerId = Guid.NewGuid();
        }

        public Guid TimerId { get; }
        public IEvent<TimerEnded> EndedEvent => _endedEvent;

        public void End()
        {
            if (_hasEnded)
                return;

            _stopwatch.Stop();

            _endedEvent.Log(new TimerEnded(TimerId, _stopwatch.Elapsed));

            _hasEnded = true;
        }

        public void Dispose()
        {
            End();
        }
    }
}
