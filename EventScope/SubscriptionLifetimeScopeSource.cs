using System;
using System.Collections.Generic;

namespace EventScope
{
    public class SubscriptionLifetimeScopeSource : IScopeSource
    {
        private readonly Dictionary<ISubscription, IScope> _activeScopes;
        private readonly object _activeScopesLock;
        private IScope _parentScope;

        public SubscriptionLifetimeScopeSource()
        {
            _activeScopes = new Dictionary<ISubscription, IScope>();
            _activeScopesLock = new object();
        }

        public bool IsActive => throw new NotImplementedException();
        public HashSet<IScope> ActiveScopes => throw new NotImplementedException();

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            _parentScope = eventArgs.StartedScope;
        }

        public void AddHandler(ISubscription subscription)
        {
            lock (_activeScopesLock)
            {
                if (_activeScopes.ContainsKey(subscription))
                    return;

                var newScope = new Scope(Guid.NewGuid(), _parentScope);
                _activeScopes.Add(subscription, newScope);
                newScope.Start();
                subscription.HandleEvent(this, new ScopeStartedEventArgs(newScope));
            }
        }

        public void RemoveHandler(ISubscription subscription)
        {
            lock (_activeScopesLock)
            {
                if (!_activeScopes.TryGetValue(subscription, out var activeScope))
                    return;

                activeScope.Stop();
            }
        }
    }
}
