using System.Collections.Generic;

namespace EventScope
{
    public class EventSubscription<TEventArgs> : ISubscription where TEventArgs : ScopedEventArgs
    {
        private readonly Subscription _subscription;

        public EventSubscription(IEventSource<TEventArgs> eventSource, IEventHandler<TEventArgs> eventHandler)
        {
            _subscription = new Subscription(
                onActivation: () => eventSource.AddHandler(eventHandler),
                onDeactivation: () => eventSource.RemoveHandler(eventHandler));
        }

        public bool Active => _subscription.Active;
        public HashSet<IScope> ActiveScopes => _subscription.ActiveScopes;

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            _subscription.HandleEvent(sender, eventArgs);
        }
    }
}
