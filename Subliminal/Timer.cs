using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class Timer : ITimer
    {
        private readonly EventLog<TimerStarted> _timerStarted;

        public Timer()
        {
            _timerStarted = new EventLog<TimerStarted>();
        }

        public RunningTimer StartNew()
        {
            var runningTimer = new RunningTimer();
            _timerStarted.Log(new TimerStarted(runningTimer.Ended));
            return runningTimer;
        }

        public IDisposable Subscribe(IObserver<TimeSpan> observer)
        {
            return _timerStarted
                .SelectMany(timerStarted => timerStarted.Ended)
                .Select(timerEnded => timerEnded.Duration)
                .Subscribe(observer);
        }
    }
}
