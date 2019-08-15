using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Subliminal
{
    public class Operation<TContext> : IOperation<TContext>
    {
        private readonly EventLog<OperationStarted<TContext>> _started;

        public Operation()
        {
            _started = new EventLog<OperationStarted<TContext>>();
        }

        public Timer StartNewTimer(TContext context)
        {
            var operationId = Guid.NewGuid();
            var timer = new Timer();
            _started.LogOccurrence(new OperationStarted<TContext>(operationId, context, timer.Stopped));
            timer.Start();
            return timer;
        }

        public IEventLog<OperationStarted<TContext>> Started => _started;

        public IEventLog<OperationCompleted<TContext>> Completed
        {
            get { return Started.SelectMany(started => started.Completed).AsEventLog(); }
        }

        public IEventLog<OperationCanceled<TContext>> Canceled
        {
            get { return Started.SelectMany(started => started.Canceled).AsEventLog(); }
        }

        public void Time(TContext context, Action<Timer> operation)
        {
            using (var timer = StartNewTimer(context))
            {
                operation(timer);
            }
        }

        public void Time(TContext context, Action operation)
        {
            Time(context, _ => operation());
        }

        public TResult Time<TResult>(TContext context, Func<Timer, TResult> operation)
        {
            using (var timer = StartNewTimer(context))
            {
                return operation(timer);
            }
        }

        public TResult Time<TResult>(TContext context, Func<TResult> operation)
        {
            return Time(context, _ => operation());
        }

        public async Task TimeAsync(TContext context, Func<Timer, Task> operation)
        {
            using (var timer = StartNewTimer(context))
            {
                await operation(timer).ConfigureAwait(false);
            }
        }

        public Task TimeAsync(TContext context, Func<Task> operation)
        {
            return TimeAsync(context, _ => operation());
        }

        public async Task<TResult> TimeAsync<TResult>(TContext context, Func<Timer, Task<TResult>> operation)
        {
            using (var timer = StartNewTimer(context))
            {
                return await operation(timer).ConfigureAwait(false);
            }
        }

        public Task<TResult> TimeAsync<TResult>(TContext context, Func<Task<TResult>> operation)
        {
            return TimeAsync(context, _ => operation());
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

        public IEventLog<OperationStarted> Started
        {
            get { return _operation.Started.Select(started => started.WithoutContext()).AsEventLog(); }
        }

        public IEventLog<OperationCompleted> Completed
        {
            get { return _operation.Completed.Select(completed => completed.WithoutContext()).AsEventLog(); }
        }

        public IEventLog<OperationCanceled> Canceled
        {
            get { return _operation.Canceled.Select(canceled => canceled.WithoutContext()).AsEventLog(); }
        }

        public void Time(Action<Timer> operation)
        {
            _operation.Time(Unit.Default, operation);
        }

        public void Time(Action operation)
        {
            _operation.Time(Unit.Default, operation);
        }

        public TResult Time<TResult>(Func<Timer, TResult> operation)
        {
            return _operation.Time(Unit.Default, operation);
        }

        public TResult Time<TResult>(Func<TResult> operation)
        {
            return _operation.Time(Unit.Default, operation);
        }

        public Task TimeAsync(Func<Timer, Task> operation)
        {
            return _operation.TimeAsync(Unit.Default, operation);
        }

        public Task TimeAsync(Func<Task> operation)
        {
            return _operation.TimeAsync(Unit.Default, operation);
        }

        public Task<TResult> TimeAsync<TResult>(Func<Timer, Task<TResult>> operation)
        {
            return _operation.TimeAsync(Unit.Default, operation);
        }

        public Task<TResult> TimeAsync<TResult>(Func<Task<TResult>> operation)
        {
            return _operation.TimeAsync(Unit.Default, operation);
        }
    }
}
