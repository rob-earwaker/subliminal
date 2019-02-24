using Subliminal.Events;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class OperationScope : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Subject<OperationEnded> _ended;
        private bool _canceled;

        public OperationScope()
        {
            OperationId = OperationId.New();
            HasStarted = false;
            HasEnded = false;

            _stopwatch = new Stopwatch();
            _ended = new Subject<OperationEnded>();
            _canceled = false;
        }

        public static OperationScope StartNew()
        {
            var operationTimer = new OperationScope();
            operationTimer.Start();
            return operationTimer;
        }

        public OperationId OperationId { get; }
        public bool HasStarted { get; private set; }
        public bool HasEnded { get; private set; }
        public TimeSpan Duration => _stopwatch.Elapsed;

        public IObservable<OperationEnded> Ended => _ended.AsObservable();
        public IObservable<OperationCompleted> Completed => Ended.Where(_ => !_canceled).Select(_ => new OperationCompleted(this));
        public IObservable<OperationCanceled> Canceled => Ended.Where(_ => _canceled).Select(_ => new OperationCanceled(this));

        public void Start()
        {
            if (HasStarted)
                return;

            _stopwatch.Start();

            HasStarted = true;
        }

        public void End()
        {
            if (!HasStarted || HasEnded)
                return;

            _stopwatch.Stop();

            _ended.OnNext(new OperationEnded(this));
            _ended.OnCompleted();

            HasEnded = true;
        }

        public void Cancel()
        {
            _canceled = true;
            End();
        }

        public void Dispose()
        {
            End();
        }
    }
}
