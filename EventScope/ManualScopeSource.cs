using System;
using System.Collections.Generic;

namespace EventScope
{
    public class ManualScopeSource : IScopeSource
    {
        private readonly ISubscription _subscription;

        private event EventHandler<ScopeStartedEventArgs> _scopeStarted;

        public ManualScopeSource(ISubscription subscription)
        {
            _subscription = subscription;
        }

        public bool IsActive => _subscription.IsActive;
        public HashSet<IScope> ActiveScopes => _subscription.ActiveScopes;

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            _subscription.HandleEvent(sender, eventArgs);
        }

        public void AddSubscription(ISubscription subscription)
        {
            _scopeStarted += subscription.HandleEvent;
        }

        public void RemoveSubscription(ISubscription subscription)
        {
            _scopeStarted -= subscription.HandleEvent;
        }

        public void RaiseScopeStartedEvent(object sender, IScope startedScope)
        {
            _scopeStarted?.Invoke(sender, new ScopeStartedEventArgs(startedScope));
        }
    }
}
