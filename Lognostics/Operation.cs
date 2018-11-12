using System;
using System.Collections.Generic;

namespace Lognostics
{
    public class Operation : IScopeSource
    {
        private readonly ConcurrentHashSet<IScope> _activeScopes;

        public Operation()
        {
            _activeScopes = new ConcurrentHashSet<IScope>();
        }

        public event EventHandler<OperationStartedEventArgs> Started;
        public event EventHandler<OperationCompletedEventArgs> Completed;

        public ICollection<IScope> ActiveScopes => _activeScopes.Snapshot();

        public IScope StartNewTimer()
        {
            var timerScope = Scope.StartNew();
            _activeScopes.Add(timerScope);
            timerScope.Stopped += TimerScopeEndedHandler;
            return timerScope;
        }

        private void TimerScopeEndedHandler(object sender, ScopeStoppedEventArgs eventArgs)
        {
            eventArgs.Scope.Stopped -= TimerScopeEndedHandler;
            _activeScopes.Remove(eventArgs.Scope);
            Completed?.Invoke(this, new OperationCompletedEventArgs(eventArgs.Scope.Duration));
        }
    }
}
