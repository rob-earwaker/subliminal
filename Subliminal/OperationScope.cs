using Subliminal.Events;
using System;
using System.Collections.Generic;

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

        public event EventHandler<OperationCompleted> Completed;
        public event EventHandler<OperationCanceled> Canceled;
        public event EventHandler<ScopeEnded> Ended;

        public void Start()
        {
            if (HasStarted)
                return;

            _scope.Ended += ScopeEndedHandler;
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

        private void ScopeEndedHandler(object sender, ScopeEnded eventArgs)
        {
            eventArgs.Scope.Ended -= ScopeEndedHandler;
            Ended?.Invoke(this, new ScopeEnded(this));

            if (_canceled)
                Canceled?.Invoke(this, new OperationCanceled(this));
            else
                Completed?.Invoke(this, new OperationCompleted(this));
        }
    }
}
