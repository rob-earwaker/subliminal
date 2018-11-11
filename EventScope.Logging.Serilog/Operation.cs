using System.Collections.Generic;

namespace EventScope.Logging.Serilog
{
    public class Operation : IEventSource<OperationCompletedEventArgs>, IScopeSource
    {
        private readonly ISubscription _subscription;
        private readonly ConcurrentHashSet<ISubscription> _scopeSubscriptions;
        private readonly ConcurrentHashSet<IEventHandler<OperationCompletedEventArgs>> _operationCompletedHandlers;

        public Operation()
        {
            _subscription = new Subscription(onActivation: () => { }, onDeactivation: () => { });
            _scopeSubscriptions = new ConcurrentHashSet<ISubscription>();
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

        public void AddHandler(ISubscription subscription)
        {
            _scopeSubscriptions.Add(subscription);
        }

        public void RemoveHandler(ISubscription subscription)
        {
            _scopeSubscriptions.Remove(subscription);
        }

        public OperationTimer StartNewTimer()
        {
            var operationTimer = new OperationTimer(_subscription);

            foreach (var scopeStartedHandler in _scopeSubscriptions.Snapshot())
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
