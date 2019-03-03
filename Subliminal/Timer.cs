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
            _timerStarted.Log(new TimerStarted(runningTimer.TimerId, runningTimer.Ended));
            return runningTimer;
        }

        public IEventLog<TimerStarted> Started => _timerStarted;

        public IEventLog<TimerEnded> Ended
        {
            get
            {
                return Started
                    .SelectMany(timerStarted => timerStarted.Ended)
                    .AsEventLog();
            }
        }

        public IMetric<TimeSpan> Duration => Ended.Select(timerEnded => timerEnded.Duration).AsMetric();
    }
}
