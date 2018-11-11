using System;
using System.Collections.Generic;

namespace EventScope
{
    public class SubscriptionLifetimeScopeSource : IScopeSource
    {
        private readonly Dictionary<IEventHandler<ScopeStartedEventArgs>, IScope> _activeScopes;
        private readonly object _activeScopesLock;
        private IScope _parentScope;

        public SubscriptionLifetimeScopeSource()
        {
            _activeScopes = new Dictionary<IEventHandler<ScopeStartedEventArgs>, IScope>();
            _activeScopesLock = new object();
        }

        public bool Active => throw new NotImplementedException();
        public HashSet<IScope> ActiveScopes => throw new NotImplementedException();

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            _parentScope = eventArgs.StartedScope;
        }

        public void AddHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            lock (_activeScopesLock)
            {
                if (_activeScopes.ContainsKey(eventHandler))
                    return;

                var newScope = new Scope(Guid.NewGuid(), _parentScope);
                _activeScopes.Add(eventHandler, newScope);
                newScope.Start();
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
