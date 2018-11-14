using System;
using System.Collections.Generic;

namespace Lognostics
{
    public class Operation : IScopeSource
    {
        private readonly ConcurrentHashSet<IScope> _activeScopes;

        public Operation()
        {
            OperationTypeId = Guid.NewGuid();
            _activeScopes = new ConcurrentHashSet<IScope>();
        }

        public Guid OperationTypeId { get; }

        public event EventHandler<OperationStartedEventArgs> Started;
        public event EventHandler<OperationCompletedEventArgs> Completed;

        public ICollection<IScope> ActiveScopes => _activeScopes.Snapshot();

        public OperationScope StartNew()
        {
            var operationScope = OperationScope.StartNew(OperationTypeId);
            Started?.Invoke(this, new OperationStartedEventArgs(operationScope));
            operationScope.Completed += OperationCompletedHandler;
            _activeScopes.Add(operationScope);
            return operationScope;
        }

        private void OperationCompletedHandler(object sender, OperationCompletedEventArgs eventArgs)
        {
            _activeScopes.Remove(eventArgs.OperationScope);
            eventArgs.OperationScope.Completed -= OperationCompletedHandler;
            Completed?.Invoke(this, eventArgs);
        }
    }
}
