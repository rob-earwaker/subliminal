using System;
using System.Collections.Generic;

namespace EventScope.Logging.Serilog
{
    public class Operation : IEventSource<OperationCompletedEventArgs>, IScopeSource
    {
        private readonly ISubscription _subscription;
        private readonly ConcurrentHashSet<IEventHandler<ScopeStartedEventArgs>> _scopeStartedHandlers;
        private readonly ConcurrentHashSet<IEventHandler<OperationCompletedEventArgs>> _operationCompletedHandlers;

        public Operation()
        {
            _subscription = new Subscription(onActivation: () => { }, onDeactivation: () => { });
            _scopeStartedHandlers = new ConcurrentHashSet<IEventHandler<ScopeStartedEventArgs>>();
            _operationCompletedHandlers = new ConcurrentHashSet<IEventHandler<OperationCompletedEventArgs>>();
        }

        public bool IsActive => _subscription.IsActive;
        public HashSet<IScope> ActiveScopes => _subscription.ActiveScopes;

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            _subscription.HandleEvent(sender, eventArgs);
        }

        public void AddHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            _operationCompletedHandlers.Add(eventHandler);
        }

        public void RemoveHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            _operationCompletedHandlers.Remove(eventHandler);
        }

        public void AddHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            _scopeStartedHandlers.Add(eventHandler);
        }

        public void RemoveHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            _scopeStartedHandlers.Remove(eventHandler);
        }

        public IScope StartNewTimer()
        {
            var operationTimer = new OperationTimer(_subscription);

            foreach (var scopeStartedHandler in _scopeStartedHandlers.Snapshot())
            {
                operationTimer.AddHandler(scopeStartedHandler);
            }

            foreach (var operationCompletedHandler in _operationCompletedHandlers.Snapshot())
            {
                operationTimer.AddHandler(operationCompletedHandler);
            }

            operationTimer.Start();

            return operationTimer;
        }
    }
}
