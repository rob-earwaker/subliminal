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

        public Execution StartNew()
        {
            var execution = new Execution(OperationId);
            _started.Log(new OperationStarted(execution.OperationId, execution.ExecutionId, execution.Ended));
            return execution;
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

        public void Execute(Action<Execution> operation)
        {
            using (var execution = StartNew())
            {
                operation(execution);
            }
        }

        public void Execute(Action operation)
        {
            Execute(_ => operation());
        }

        public TResult Execute<TResult>(Func<Execution, TResult> operation)
        {
            using (var execution = StartNew())
            {
                return operation(execution);
            }
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return Execute(_ => operation());
        }

        public async Task ExecuteAsync(Func<Execution, Task> operation)
        {
            using (var execution = StartNew())
            {
                await operation(execution);
            }
        }

        public Task ExecuteAsync(Func<Task> operation)
        {
            return ExecuteAsync(_ => operation());
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Execution, Task<TResult>> operation)
        {
            using (var execution = StartNew())
            {
                return await operation(execution);
            }
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            return ExecuteAsync(_ => operation());
        }
    }
}
