using System;
using System.Collections.Generic;
using System.Linq;

namespace EventScope
{
    public class Subscription : ISubscription
    {
        private readonly Action _onActivation;
        private readonly Action _onDeactivation;
        private readonly HashSet<IScope> _activeScopes;
        private readonly object _activeScopesLock;
        private readonly IEventHandler<ScopeEndedEventArgs> _scopeEndedEventHandler;

        public Subscription(Action onActivation, Action onDeactivation)
        {
            _onActivation = onActivation;
            _onDeactivation = onDeactivation;
            _activeScopes = new HashSet<IScope>();
            _activeScopesLock = new object();
            _scopeEndedEventHandler = new DelegateEventHandler<ScopeEndedEventArgs>(Unsubscribe);
        }

        public bool Active
        {
            get
            {
                lock (_activeScopesLock)
                {
                    return _activeScopes.Any();
                }
            }
        }

        public HashSet<IScope> ActiveScopes
        {
            get
            {
                lock (_activeScopesLock)
                {
                    return new HashSet<IScope>(_activeScopes);
                }
            }
        }

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            lock (_activeScopesLock)
            {
                if (!_activeScopes.Any())
                    _onActivation?.Invoke();

                _activeScopes.Add(eventArgs.StartedScope);
                eventArgs.StartedScope.ScopeEnded.AddHandler(_scopeEndedEventHandler);
            }
        }

        private void Unsubscribe(object sender, ScopeEndedEventArgs eventArgs)
        {
            lock (_activeScopesLock)
            {
                eventArgs.EventScope.ScopeEnded.RemoveHandler(_scopeEndedEventHandler);
                _activeScopes.Remove(eventArgs.EventScope);

                if (!_activeScopes.Any())
                    _onDeactivation?.Invoke();
            }
        }
    }
}
