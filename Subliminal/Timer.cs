using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Subliminal
{
    public class Timer : ITimer
    {
        private readonly EventLog<TimerStarted> _timerStarted;
        private readonly ITimer _derivedTimer;

        public Timer()
        {
            _timerStarted = new EventLog<TimerStarted>();
            _derivedTimer = _timerStarted.Events
                .SelectMany(timerStarted => timerStarted.Ended)
                .Select(timerEnded => timerEnded.Duration)
                .AsTimer();
        }

        public Guid TimerId => _derivedTimer.TimerId;

        public IObservable<TimeSpan> Durations => _derivedTimer.Durations;

        public RunningTimer StartNew()
        {
            var runningTimer = new RunningTimer();
            _timerStarted.Log(new TimerStarted(runningTimer.Ended));
            return runningTimer;
        }

        public void Time(Action<RunningTimer> operation)
        {
            using (var runningTimer = StartNew())
            {
                operation(runningTimer);
            }
        }

        public void Time(Action operation)
        {
            Time(_ => operation());
        }

        public TResult Time<TResult>(Func<RunningTimer, TResult> operation)
        {
            using (var runningTimer = StartNew())
            {
                return operation(runningTimer);
            }
        }

        public TResult Time<TResult>(Func<TResult> operation)
        {
            return Time(_ => operation());
        }

        public async Task TimeAsync(Func<RunningTimer, Task> operation)
        {
            using (var runningTimer = StartNew())
            {
                await operation(runningTimer);
            }
        }

        public Task TimeAsync(Func<Task> operation)
        {
            return TimeAsync(_ => operation());
        }

        public async Task<TResult> TimeAsync<TResult>(Func<RunningTimer, Task<TResult>> operation)
        {
            using (var runningTimer = StartNew())
            {
                return await operation(runningTimer);
            }
        }

        public Task<TResult> TimeAsync<TResult>(Func<Task<TResult>> operation)
        {
            return TimeAsync(_ => operation());
        }
    }
}
