using System;
using System.Collections.Generic;

namespace EventScope
{
    public class SubscriptionLifetimeScopeSource : IEventSource<ScopeStartedEventArgs>
    {
        private readonly Dictionary<IEventHandler<ScopeStartedEventArgs>, IScope> _activeScopes;
        private readonly object _activeScopesLock;

        public SubscriptionLifetimeScopeSource()
        {
            _activeScopes = new Dictionary<IEventHandler<ScopeStartedEventArgs>, IScope>();
            _activeScopesLock = new object();
        }

        public void AddHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            lock (_activeScopesLock)
            {
                if (_activeScopes.ContainsKey(eventHandler))
                    return;

                var newScope = new Scope(Guid.NewGuid());
                _activeScopes.Add(eventHandler, newScope);
                eventHandler.HandleEvent(this, new ScopeStartedEventArgs(newScope));
            }
        }

        public void RemoveHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            lock (_activeScopesLock)
            {
                if (!_activeScopes.TryGetValue(eventHandler, out var activeScope))
                    return;

                activeScope.Stop();
            }
        }
    }
}
