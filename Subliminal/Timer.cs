using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class Timer : ITimer
    {
        private readonly EventLog<TimerStarted> _timerStarted;
        private readonly ITimer _timer;

        public Timer()
        {
            _timerStarted = new EventLog<TimerStarted>();
            _timer = _timerStarted
                .SelectMany(timerStarted => timerStarted.Ended)
                .Select(timerEnded => timerEnded.Duration)
                .AsTimer();
        }

        public RunningTimer StartNew()
        {
            var runningTimer = new RunningTimer();
            _timerStarted.Log(new TimerStarted(runningTimer.Ended));
            return runningTimer;
        }

        public Guid TimerId => _timer.TimerId;

        public IDisposable Subscribe(IObserver<TimeSpan> observer)
        {
            return _timer.Subscribe(observer);
        }
    }
}
