using System;
using System.Collections.Generic;

namespace EventScope.Logging.Serilog
{
    public class Operation : IEventSource<OperationCompletedEventArgs>, IScopeSource
    {
        private readonly ManualEventSource<ScopeStartedEventArgs> _scopeStarted;
        private readonly DelegateEventHandler<ScopeEndedEventArgs> _operationScopeEndedHandler;
        private readonly ISubscription _subscription;

        private event EventHandler<OperationCompletedEventArgs> _operationCompleted;

        public Operation()
        {
            _scopeStarted = new ManualEventSource<ScopeStartedEventArgs>();
            _operationScopeEndedHandler = new DelegateEventHandler<ScopeEndedEventArgs>(RaiseOperationCompleted);
            _subscription = new Subscription(onActivation: () => { }, onDeactivation: () => { });
        }

        public bool Active => _subscription.Active;
        public HashSet<IScope> ActiveScopes => _subscription.ActiveScopes;

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            _subscription.HandleEvent(sender, eventArgs);
        }

        public void AddHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            _operationCompleted += eventHandler.HandleEvent;
        }

        public void RemoveHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            _operationCompleted -= eventHandler.HandleEvent;
        }

        public void AddHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            _scopeStarted.AddHandler(eventHandler);
        }

        public void RemoveHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            _scopeStarted.RemoveHandler(eventHandler);
        }

        public IScope StartNewTimer()
        {
            var newScope = new Scope(Guid.NewGuid(), Scope.RootScope);
            newScope.ScopeEnded.AddHandler(_operationScopeEndedHandler);
            newScope.Start();
            _scopeStarted.RaiseEvent(this, new ScopeStartedEventArgs(newScope));
            return newScope;
        }

        private void RaiseOperationCompleted(object sender, ScopeEndedEventArgs eventArgs)
        {
            foreach (var activeScope in _subscription.ActiveScopes)
            {
                _operationCompleted?.Invoke(this, new OperationCompletedEventArgs(activeScope, eventArgs.EndedScope.Duration));
            }
        }
    }
}
