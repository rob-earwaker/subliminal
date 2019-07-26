using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Subliminal
{
    public class Operation : IOperation
    {
        private readonly EventLog<OperationStarted> _started;

        public Operation()
        {
            OperationId = Guid.NewGuid();
            _started = new EventLog<OperationStarted>();
        }

        public Guid OperationId { get; }

        public ExecutionTimer StartNew()
        {
            var executionTimer = new ExecutionTimer(OperationId);
            _started.Log(new OperationStarted(executionTimer.OperationId, executionTimer.ExecutionId, executionTimer.Ended));
            return executionTimer;
        }

        public IEventLog<OperationStarted> Started => _started;

        public IEventLog<OperationEnded> Ended
        {
            get { return Started.Events.SelectMany(operationStarted => operationStarted.Ended).AsEventLog(); }
        }

        public IEventLog<OperationCompleted> Completed
        {
            get { return Started.Events.SelectMany(operationStarted => operationStarted.Completed).AsEventLog(); }
        }

        public IEventLog<OperationCanceled> Canceled
        {
            get { return Started.Events.SelectMany(operationStarted => operationStarted.Canceled).AsEventLog(); }
        }

        public void Execute(Action<ExecutionTimer> operation)
        {
            using (var executionTimer = StartNew())
            {
                operation(executionTimer);
            }
        }

        public void Execute(Action operation)
        {
            Execute(_ => operation());
        }

        public TResult Execute<TResult>(Func<ExecutionTimer, TResult> operation)
        {
            using (var executionTimer = StartNew())
            {
                return operation(executionTimer);
            }
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return Execute(_ => operation());
        }

        public async Task ExecuteAsync(Func<ExecutionTimer, Task> operation)
        {
            using (var executionTimer = StartNew())
            {
                await operation(executionTimer);
            }
        }

        public Task ExecuteAsync(Func<Task> operation)
        {
            return ExecuteAsync(_ => operation());
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<ExecutionTimer, Task<TResult>> operation)
        {
            using (var executionTimer = StartNew())
            {
                return await operation(executionTimer);
            }
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            return ExecuteAsync(_ => operation());
        }
    }
}
