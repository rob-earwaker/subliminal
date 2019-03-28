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
            _started = new EventLog<OperationStarted>();
        }

        public RunningOperation StartNew()
        {
            var runningOperation = new RunningOperation();
            _started.Log(new OperationStarted(runningOperation.OperationId, runningOperation.Ended));
            return runningOperation;
        }

        public IEventLog<OperationStarted> Started => _started;

        public IEventLog<OperationEnded> Ended
        {
            get { return Started.SelectMany(operationStarted => operationStarted.Ended).AsEventLog(); }
        }

        public IEventLog<OperationCompleted> Completed
        {
            get { return Started.SelectMany(operationStarted => operationStarted.Completed).AsEventLog(); }
        }

        public IEventLog<OperationCanceled> Canceled
        {
            get { return Started.SelectMany(operationStarted => operationStarted.Canceled).AsEventLog(); }
        }

        public void Execute(Action<RunningOperation> operation)
        {
            using (var runningOperation = StartNew())
            {
                operation(runningOperation);
            }
        }

        public void Execute(Action operation)
        {
            Execute(_ => operation());
        }

        public TResult Execute<TResult>(Func<RunningOperation, TResult> operation)
        {
            using (var runningOperation = StartNew())
            {
                return operation(runningOperation);
            }
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return Execute(_ => operation());
        }

        public async Task ExecuteAsync(Func<RunningOperation, Task> operation)
        {
            using (var runningOperation = StartNew())
            {
                await operation(runningOperation);
            }
        }

        public Task ExecuteAsync(Func<Task> operation)
        {
            return ExecuteAsync(_ => operation());
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<RunningOperation, Task<TResult>> operation)
        {
            using (var runningOperation = StartNew())
            {
                return await operation(runningOperation);
            }
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            return ExecuteAsync(_ => operation());
        }
    }
}
