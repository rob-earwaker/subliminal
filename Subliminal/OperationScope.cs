using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace Subliminal
{
    public class OperationScope : IScope
    {
        private readonly IScope _scope;
        private bool _canceled;

        public OperationScope()
        {
            _scope = new Scope();
            _canceled = false;
        }

        public static OperationScope StartNew()
        {
            var operationTimer = new OperationScope();
            operationTimer.Start();
            return operationTimer;
        }

        public bool HasStarted => _scope.HasStarted;
        public bool HasEnded => _scope.HasEnded;
        public IReadOnlyDictionary<string, object> Context => _scope.Context;
        public TimeSpan Duration => _scope.Duration;
        public IObservable<Unit> Ended => _scope.Ended;

        public IObservable<Unit> Completed => _scope.Ended.Where(_ => !_canceled);
        public IObservable<Unit> Canceled => _scope.Ended.Where(_ => _canceled);

        public void Start()
        {
            _scope.Start();
        }

        public void Cancel()
        {
            _canceled = true;
            End();
        }

        public void End()
        {
            _scope.End();
        }

        public void AddContext(string contextKey, object value)
        {
            _scope.AddContext(contextKey, value);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
