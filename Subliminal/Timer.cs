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
            _timerStarted.Log(new TimerStarted(runningTimer.TimerId, runningTimer.EndedEvent));
            return runningTimer;
        }

        public IEventLog<TimerStarted> TimerStarted => _timerStarted;

        public IEventLog<TimerEnded> TimerEnded
        {
            get
            {
                return _timerStarted
                    .SelectMany(timerStarted => timerStarted.EndedEvent)
                    .AsEventLog();
            }
        }

        public IMetric<TimeSpan> Duration => TimerEnded.Select(timerEnded => timerEnded.Duration).AsMetric();
    }
}
