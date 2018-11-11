using System;
using System.Collections.Generic;

namespace EventScope
{
    public class Subscription : ISubscription
    {
        private readonly Action _onActivation;
        private readonly Action _onDeactivation;
        private readonly object _activationLock;
        private readonly ConcurrentHashSet<IScope> _activeScopes;
        private readonly IEventHandler<ScopeEndedEventArgs> _scopeEndedEventHandler;

        public Subscription(Action onActivation, Action onDeactivation)
        {
            _onActivation = onActivation;
            _onDeactivation = onDeactivation;
            _activationLock = new object();
            _activeScopes = new ConcurrentHashSet<IScope>();
            _scopeEndedEventHandler = new DelegateEventHandler<ScopeEndedEventArgs>(Unsubscribe);
        }

        public bool IsActive => _activeScopes.Any();
        public HashSet<IScope> ActiveScopes => _activeScopes.Snapshot();

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            lock (_activationLock)
            {
                if (!_activeScopes.Any())
                    _onActivation?.Invoke();

                _activeScopes.Add(eventArgs.StartedScope);
                eventArgs.StartedScope.ScopeEnded.AddHandler(_scopeEndedEventHandler);
            }
        }

        private void Unsubscribe(object sender, ScopeEndedEventArgs eventArgs)
        {
            lock (_activationLock)
            {
                eventArgs.EventScope.ScopeEnded.RemoveHandler(_scopeEndedEventHandler);
                _activeScopes.Remove(eventArgs.EndedScope);

                if (!_activeScopes.Any())
                    _onDeactivation?.Invoke();
            }
        }
    }
}
