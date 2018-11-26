using Lognostics.Events;
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

        public event EventHandler<OperationStarted> Started;
        public event EventHandler<OperationCompleted> Completed;

        public ICollection<IScope> ActiveScopes => _activeScopes.Snapshot();

        public OperationScope StartNew()
        {
            var operationScope = OperationScope.StartNew();
            Started?.Invoke(this, new OperationStarted(operationScope));
            operationScope.Completed += OperationCompletedHandler;
            _activeScopes.Add(operationScope);
            return operationScope;
        }

        private void OperationCompletedHandler(object sender, OperationCompleted eventArgs)
        {
            _activeScopes.Remove(eventArgs.OperationScope);
            eventArgs.OperationScope.Completed -= OperationCompletedHandler;
            Completed?.Invoke(this, eventArgs);
        }
    }
}
