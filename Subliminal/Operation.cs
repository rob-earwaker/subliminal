using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Subliminal
{
    public class Operation<TContext> : IOperation<TContext>
    {
        private readonly EventLog<StartedOperation<TContext>> _started;

        public Operation()
        {
            _started = new EventLog<StartedOperation<TContext>>();
        }

        public Timer StartNewTimer(TContext context)
        {
            var operationId = Guid.NewGuid();
            var timer = new Timer();

            var operationEnded = timer.Ended
                .Select(timerEnded => new EndedOperation<TContext>(
                    operationId, context, timerEnded.Duration, timerEnded.WasCanceled))
                .AsEvent();

            _started.LogOccurrence(new StartedOperation<TContext>(operationId, context, operationEnded));

            timer.Start();
            return timer;
        }

        public IEventLog<StartedOperation<TContext>> Started => _started;

        public IEventLog<EndedOperation<TContext>> Ended
        {
            get { return Started.SelectMany(operation => operation.Ended).AsEventLog(); }
        }

        public IEventLog<CompletedOperation<TContext>> Completed
        {
            get { return Started.SelectMany(operation => operation.Completed).AsEventLog(); }
        }

        public IEventLog<CanceledOperation<TContext>> Canceled
        {
            get { return Started.SelectMany(operation => operation.Canceled).AsEventLog(); }
        }

        public void Time(Action<Timer> operation, TContext context)
        {
            using (var timer = StartNewTimer(context))
            {
                operation(timer);
            }
        }

        public void Time(Action operation, TContext context)
        {
            Time(_ => operation(), context);
        }

        public TResult Time<TResult>(Func<Timer, TResult> operation, TContext context)
        {
            using (var timer = StartNewTimer(context))
            {
                return operation(timer);
            }
        }

        public TResult Time<TResult>(Func<TResult> operation, TContext context)
        {
            return Time(_ => operation(), context);
        }

        public async Task TimeAsync(Func<Timer, Task> operation, TContext context)
        {
            using (var timer = StartNewTimer(context))
            {
                await operation(timer).ConfigureAwait(false);
            }
        }

        public Task TimeAsync(Func<Task> operation, TContext context)
        {
            return TimeAsync(_ => operation(), context);
        }

        public async Task<TResult> TimeAsync<TResult>(Func<Timer, Task<TResult>> operation, TContext context)
        {
            using (var timer = StartNewTimer(context))
            {
                return await operation(timer).ConfigureAwait(false);
            }
        }

        public Task<TResult> TimeAsync<TResult>(Func<Task<TResult>> operation, TContext context)
        {
            return TimeAsync(_ => operation(), context);
        }
    }

    public class Operation : IOperation
    {
        private readonly Operation<Unit> _operation;

        public Operation()
        {
            _operation = new Operation<Unit>();
        }

        public Timer StartNewTimer()
        {
            return _operation.StartNewTimer(Unit.Default);
        }

        public IEventLog<StartedOperation> Started
        {
            get { return _operation.Started.Select(operation => operation.WithoutContext()).AsEventLog(); }
        }

        public IEventLog<EndedOperation> Ended
        {
            get { return _operation.Ended.Select(operation => operation.WithoutContext()).AsEventLog(); }
        }

        public IEventLog<CompletedOperation> Completed
        {
            get { return _operation.Completed.Select(operation => operation.WithoutContext()).AsEventLog(); }
        }

        public IEventLog<CanceledOperation> Canceled
        {
            get { return _operation.Canceled.Select(operation => operation.WithoutContext()).AsEventLog(); }
        }

        public void Time(Action<Timer> operation)
        {
            _operation.Time(operation, Unit.Default);
        }

        public void Time(Action operation)
        {
            _operation.Time(operation, Unit.Default);
        }

        public TResult Time<TResult>(Func<Timer, TResult> operation)
        {
            return _operation.Time(operation, Unit.Default);
        }

        public TResult Time<TResult>(Func<TResult> operation)
        {
            return _operation.Time(operation, Unit.Default);
        }

        public Task TimeAsync(Func<Timer, Task> operation)
        {
            return _operation.TimeAsync(operation, Unit.Default);
        }

        public Task TimeAsync(Func<Task> operation)
        {
            return _operation.TimeAsync(operation, Unit.Default);
        }

        public Task<TResult> TimeAsync<TResult>(Func<Timer, Task<TResult>> operation)
        {
            return _operation.TimeAsync(operation, Unit.Default);
        }

        public Task<TResult> TimeAsync<TResult>(Func<Task<TResult>> operation)
        {
            return _operation.TimeAsync(operation, Unit.Default);
        }
    }
}
