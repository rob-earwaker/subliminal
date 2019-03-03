using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class RunningOperation : IDisposable
    {
        private readonly RunningTimer _runningTimer;
        private bool _canceled;

        public RunningOperation()
        {
            _runningTimer = new RunningTimer();
            _canceled = false;

            OperationId = Guid.NewGuid();
        }

        public Guid OperationId { get; }

        public IEvent<OperationEnded> Ended
        {
            get
            {
                return _runningTimer.Ended
                    .Select(timerEnded => new OperationEnded(OperationId, timerEnded.Duration, _canceled))
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
