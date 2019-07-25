using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class Execution : IDisposable
    {
        private readonly RunningTimer _runningTimer;
        private bool _canceled;

        public Execution(Guid operationId)
        {
            OperationId = operationId;
            ExecutionId = Guid.NewGuid();

            _runningTimer = new RunningTimer();
            _canceled = false;

        }

        public Guid OperationId { get; }
        public Guid ExecutionId { get; }

        public IEvent<OperationEnded> Ended
        {
            get
            {
                return _runningTimer.Ended
                    .Select(timerEnded => new OperationEnded(OperationId, ExecutionId, timerEnded.Duration, _canceled))
                    .AsEvent();
            }
        }

        public void Cancel()
        {
            _canceled = true;
            End();
        }

        public void End()
        {
            _runningTimer.End();
        }

        public void Dispose()
        {
            _runningTimer.Dispose();
        }
    }
}
