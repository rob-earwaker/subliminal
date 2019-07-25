using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Subliminal
{
    public class Execution : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<OperationEnded> _ended;
        private bool _hasEnded;
        private bool _wasCanceled;

        public Execution(Guid operationId)
        {
            OperationId = operationId;
            ExecutionId = Guid.NewGuid();

            _stopwatch = Stopwatch.StartNew();
            _ended = new Event<OperationEnded>();
            _hasEnded = false;
            _wasCanceled = false;

        }

        public Guid OperationId { get; }
        public Guid ExecutionId { get; }

        public IEvent<OperationEnded> Ended => _ended;

        public void Cancel()
        {
            _wasCanceled = true;

            End();
        }

        public void End()
        {
            if (_hasEnded)
                return;

            _stopwatch.Stop();

            _ended.Raise(new OperationEnded(OperationId, ExecutionId, _stopwatch.Elapsed, _wasCanceled));

            _hasEnded = true;
        }

        public void Dispose()
        {
            End();
        }
    }
}
