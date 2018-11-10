using System.Collections.Generic;
using System.Linq;

namespace EventScope
{
    public class Subscription<TEventArgs> : IEventHandler<ScopeStartedEventArgs> where TEventArgs : ScopedEventArgs
    {
        private readonly IEventSource<TEventArgs> _eventSource;
        private readonly IEventHandler<TEventArgs> _eventHandler;
        private readonly HashSet<IScope> _activeScopes;
        private readonly object _activeScopesLock;
        private readonly IEventHandler<ScopeEndedEventArgs> _scopeEndedEventHandler;

        public Subscription(IEventSource<TEventArgs> eventSource, IEventHandler<TEventArgs> eventHandler)
        {
            _eventSource = eventSource;
            _eventHandler = eventHandler;
            _activeScopes = new HashSet<IScope>();
            _activeScopesLock = new object();
            _scopeEndedEventHandler = new DelegateEventHandler<ScopeEndedEventArgs>(Unsubscribe);
        }

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            lock (_activeScopesLock)
            {
                if (!_activeScopes.Any())
                    _eventSource.AddHandler(_eventHandler);

                _activeScopes.Add(eventArgs.Scope);
                eventArgs.Scope.ScopeEnded.AddHandler(_scopeEndedEventHandler);
            }
        }

        private void Unsubscribe(object sender, ScopeEndedEventArgs eventArgs)
        {
            lock (_activeScopesLock)
            {
                eventArgs.Scope.ScopeEnded.RemoveHandler(_scopeEndedEventHandler);
                _activeScopes.Remove(eventArgs.Scope);

                if (!_activeScopes.Any())
                    _eventSource.RemoveHandler(_eventHandler);
            }
        }
    }
}
