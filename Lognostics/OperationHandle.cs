using System;
using System.Collections.Generic;

namespace Lognostics
{
    public class OperationHandle : IScopeSource
    {
        private readonly ConcurrentHashSet<IScope> _activeScopes;

        public OperationHandle()
        {
            Id = Guid.NewGuid();
            _activeScopes = new ConcurrentHashSet<IScope>();
        }

        public Guid Id { get; }

        public event EventHandler<OperationStartedEventArgs> Started;
        public event EventHandler<OperationCompletedEventArgs> Completed;

        public ICollection<IScope> ActiveScopes => _activeScopes.Snapshot();

        public Operation StartNew()
        {
            var operationScope = Operation.StartNew();
            Started?.Invoke(this, new OperationStartedEventArgs(operationScope));
            operationScope.Ended += OperationScopeEndedHandler;
            _activeScopes.Add(operationScope);
            return operationScope;
        }

        private void OperationScopeEndedHandler(object sender, ScopeEndedEventArgs eventArgs)
        {
            _activeScopes.Remove(eventArgs.Scope);
            eventArgs.Scope.Ended -= OperationScopeEndedHandler;
            Completed?.Invoke(this, new OperationCompletedEventArgs(eventArgs.Scope.Duration));
        }
    }
}
