using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Subliminal
{
    public class Operation : IOperation
    {
        private readonly EventLog<StartedOperation> _started;

        public Operation()
        {
            _started = new EventLog<StartedOperation>();
        }

        public Timer StartNewTimer()
        {
            var timer = new Timer();
            _started.LogOccurrence(new StartedOperation(timer.OperationId, timer.Ended));
            timer.Start();
            return timer;
        }

        public IEventLog<StartedOperation> Started => _started;
        public IEventLog<EndedOperation> Ended => Started.SelectMany(operation => operation.Ended).AsEventLog();
        public IEventLog<CompletedOperation> Completed => Started.SelectMany(operation => operation.Completed).AsEventLog();
        public IEventLog<CanceledOperation> Canceled => Started.SelectMany(operation => operation.Canceled).AsEventLog();

        public void Time(Action<Timer> operation)
        {
            using (var timer = StartNewTimer())
            {
                operation(timer);
            }
        }

        public void Time(Action operation)
        {
            Time(_ => operation());
        }

        public TResult Time<TResult>(Func<Timer, TResult> operation)
        {
            using (var timer = StartNewTimer())
            {
                return operation(timer);
            }
        }

        public TResult Time<TResult>(Func<TResult> operation)
        {
            return Time(_ => operation());
        }

        public async Task TimeAsync(Func<Timer, Task> operation)
        {
            using (var timer = StartNewTimer())
            {
                await operation(timer);
            }
        }

        public Task TimeAsync(Func<Task> operation)
        {
            return TimeAsync(_ => operation());
        }

        public async Task<TResult> TimeAsync<TResult>(Func<Timer, Task<TResult>> operation)
        {
            using (var timer = StartNewTimer())
            {
                return await operation(timer);
            }
        }

        public Task<TResult> TimeAsync<TResult>(Func<Task<TResult>> operation)
        {
            return TimeAsync(_ => operation());
        }
    }
}
