using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Subliminal
{
    /// <summary>
    /// An operation that can be used to start operation timers
    /// and that emits operation metrics.
    /// </summary>
    public sealed class Operation<TContext> : IOperation<TContext>
    {
        private readonly EventLog<OperationStarted<TContext>> _started;

        /// <summary>
        /// Creates an operation that can be used to start operation timers
        /// and that emits operation metrics.
        /// </summary>
        public Operation()
        {
            _started = new EventLog<OperationStarted<TContext>>();
        }

        /// <summary>
        /// Starts a new timer that can be used to capture completion or cancellation
        /// of the operation in addition to its duration. This information will be
        /// emitted to all observers of the operation metrics.
        /// </summary>
        public Timer StartNewTimer(TContext context)
        {
            var timer = new Timer();
            _started.LogOccurrence(new OperationStarted<TContext>(OperationId.New(), context, timer.Stopped));
            timer.Start();
            return timer;
        }

        /// <summary>
        /// An event log that emits an event every time a new operation timer is started.
        /// </summary>
        public IEventLog<OperationStarted<TContext>> Started => _started;

        /// <summary>
        /// An event log that emits an event every time an operation timer is completed.
        /// </summary>
        public IEventLog<OperationCompleted<TContext>> Completed
        {
            get { return Started.SelectMany(started => started.Completed).AsEventLog(); }
        }

        /// <summary>
        /// An event log that emits an event every time an operation timer is canceled.
        /// </summary>
        public IEventLog<OperationCanceled<TContext>> Canceled
        {
            get { return Started.SelectMany(started => started.Canceled).AsEventLog(); }
        }

        /// <summary>
        /// Time an operation using a new timer that can be used to capture completion
        /// or cancellation of the operation in addition to its duration. This information
        /// will be emitted to all observers of the operation metrics.
        /// </summary>
        public void Time(TContext context, Action<Timer> operation)
        {
            using (var timer = StartNewTimer(context))
            {
                operation(timer);
            }
        }

        /// <summary>
        /// Time an operation using a new timer that will complete when the operation finishes
        /// and will emit timing information to all observers of the operation metrics.
        /// </summary>
        public void Time(TContext context, Action operation)
        {
            Time(context, _ => operation());
        }

        /// <summary>
        /// Time an operation using a new timer that can be used to capture completion
        /// or cancellation of the operation in addition to its duration. This information
        /// will be emitted to all observers of the operation metrics.
        /// </summary>
        public TResult Time<TResult>(TContext context, Func<Timer, TResult> operation)
        {
            using (var timer = StartNewTimer(context))
            {
                return operation(timer);
            }
        }

        /// <summary>
        /// Time an operation using a new timer that will complete when the operation finishes
        /// and will emit timing information to all observers of the operation metrics.
        /// </summary>
        public TResult Time<TResult>(TContext context, Func<TResult> operation)
        {
            return Time(context, _ => operation());
        }

        /// <summary>
        /// Time an operation using a new timer that can be used to capture completion
        /// or cancellation of the operation in addition to its duration. This information
        /// will be emitted to all observers of the operation metrics.
        /// </summary>
        public async Task TimeAsync(TContext context, Func<Timer, Task> operation)
        {
            using (var timer = StartNewTimer(context))
            {
                await operation(timer).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Time an operation using a new timer that will complete when the operation finishes
        /// and will emit timing information to all observers of the operation metrics.
        /// </summary>
        public Task TimeAsync(TContext context, Func<Task> operation)
        {
            return TimeAsync(context, _ => operation());
        }

        /// <summary>
        /// Time an operation using a new timer that can be used to capture completion
        /// or cancellation of the operation in addition to its duration. This information
        /// will be emitted to all observers of the operation metrics.
        /// </summary>
        public async Task<TResult> TimeAsync<TResult>(TContext context, Func<Timer, Task<TResult>> operation)
        {
            using (var timer = StartNewTimer(context))
            {
                return await operation(timer).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Time an operation using a new timer that will complete when the operation finishes
        /// and will emit timing information to all observers of the operation metrics.
        /// </summary>
        public Task<TResult> TimeAsync<TResult>(TContext context, Func<Task<TResult>> operation)
        {
            return TimeAsync(context, _ => operation());
        }
    }

    /// <summary>
    /// An operation that can be used to start operation timers
    /// and that emits operation metrics.
    /// </summary>
    public sealed class Operation : IOperation
    {
        private readonly Operation<Unit> _operation;

        /// <summary>
        /// Creates an operation that can be used to start operation timers
        /// and that emits operation metrics.
        /// </summary>
        public Operation()
        {
            _operation = new Operation<Unit>();
        }

        /// <summary>
        /// Starts a new timer that can be used to capture completion or cancellation
        /// of the operation in addition to its duration. This information will be
        /// emitted to all observers of the operation metrics.
        /// </summary>
        public Timer StartNewTimer()
        {
            return _operation.StartNewTimer(Unit.Default);
        }

        /// <summary>
        /// An event log that emits an event every time a new operation timer is started.
        /// </summary>
        public IEventLog<OperationStarted> Started
        {
            get { return _operation.Started.Select(started => started.WithoutContext()).AsEventLog(); }
        }

        /// <summary>
        /// An event log that emits an event every time an operation timer is completed.
        /// </summary>
        public IEventLog<OperationCompleted> Completed
        {
            get { return _operation.Completed.Select(completed => completed.WithoutContext()).AsEventLog(); }
        }

        /// <summary>
        /// An event log that emits an event every time an operation timer is canceled.
        /// </summary>
        public IEventLog<OperationCanceled> Canceled
        {
            get { return _operation.Canceled.Select(canceled => canceled.WithoutContext()).AsEventLog(); }
        }

        /// <summary>
        /// Time an operation using a new timer that can be used to capture completion
        /// or cancellation of the operation in addition to its duration. This information
        /// will be emitted to all observers of the operation metrics.
        /// </summary>
        public void Time(Action<Timer> operation)
        {
            _operation.Time(Unit.Default, operation);
        }

        /// <summary>
        /// Time an operation using a new timer that will complete when the operation finishes
        /// and will emit timing information to all observers of the operation metrics.
        /// </summary>
        public void Time(Action operation)
        {
            _operation.Time(Unit.Default, operation);
        }

        /// <summary>
        /// Time an operation using a new timer that can be used to capture completion
        /// or cancellation of the operation in addition to its duration. This information
        /// will be emitted to all observers of the operation metrics.
        /// </summary>
        public TResult Time<TResult>(Func<Timer, TResult> operation)
        {
            return _operation.Time(Unit.Default, operation);
        }

        /// <summary>
        /// Time an operation using a new timer that will complete when the operation finishes
        /// and will emit timing information to all observers of the operation metrics.
        /// </summary>
        public TResult Time<TResult>(Func<TResult> operation)
        {
            return _operation.Time(Unit.Default, operation);
        }

        /// <summary>
        /// Time an operation using a new timer that can be used to capture completion
        /// or cancellation of the operation in addition to its duration. This information
        /// will be emitted to all observers of the operation metrics.
        /// </summary>
        public Task TimeAsync(Func<Timer, Task> operation)
        {
            return _operation.TimeAsync(Unit.Default, operation);
        }

        /// <summary>
        /// Time an operation using a new timer that will complete when the operation finishes
        /// and will emit timing information to all observers of the operation metrics.
        /// </summary>
        public Task TimeAsync(Func<Task> operation)
        {
            return _operation.TimeAsync(Unit.Default, operation);
        }

        /// <summary>
        /// Time an operation using a new timer that can be used to capture completion
        /// or cancellation of the operation in addition to its duration. This information
        /// will be emitted to all observers of the operation metrics.
        /// </summary>
        public Task<TResult> TimeAsync<TResult>(Func<Timer, Task<TResult>> operation)
        {
            return _operation.TimeAsync(Unit.Default, operation);
        }

        /// <summary>
        /// Time an operation using a new timer that will complete when the operation finishes
        /// and will emit timing information to all observers of the operation metrics.
        /// </summary>
        public Task<TResult> TimeAsync<TResult>(Func<Task<TResult>> operation)
        {
            return _operation.TimeAsync(Unit.Default, operation);
        }
    }
}
