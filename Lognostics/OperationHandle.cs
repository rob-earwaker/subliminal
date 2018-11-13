using System;
using System.Collections.Generic;

namespace Lognostics
{
    public class OperationHandle :
        IEventSource<OperationStartedEventArgs>, IEventSource<OperationCompletedEventArgs>, IScopeSource
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

        public void AddHandler(IEventHandler<OperationStartedEventArgs> eventHandler)
        {
            AddHandler(eventHandler.HandleEvent);
        }

        public void AddHandler(EventHandler<OperationStartedEventArgs> eventHandler)
        {
            Started += eventHandler;
        }

        public void RemoveHandler(IEventHandler<OperationStartedEventArgs> eventHandler)
        {
            RemoveHandler(eventHandler);
        }

        public void RemoveHandler(EventHandler<OperationStartedEventArgs> eventHandler)
        {
            Started -= eventHandler;
        }

        public void AddHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            AddHandler(eventHandler.HandleEvent);
        }

        public void AddHandler(EventHandler<OperationCompletedEventArgs> eventHandler)
        {
            Completed += eventHandler;
        }

        public void RemoveHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            RemoveHandler(eventHandler);
        }

        public void RemoveHandler(EventHandler<OperationCompletedEventArgs> eventHandler)
        {
            Completed -= eventHandler;
        }
    }
}
