using System.Collections.Generic;

namespace EventScope
{
    public static class EventAggregator
    {
        public static IScopedEventHandler<TEvent> WithHandler<TEvent>(IScopedEventHandler<TEvent[]> eventHandler)
        {
            return new EventAggregator<TEvent>(eventHandler);
        }
    }

    public class EventAggregator<TEvent> : IScopedEventHandler<TEvent>
    {
        private readonly IScopedEventHandler<TEvent[]> _eventHandler;
        private readonly Dictionary<IScope, List<TEvent>> _collectedEventArgs;
        private readonly object _collectedEventArgsLock;

        public EventAggregator(IScopedEventHandler<TEvent[]> eventHandler)
        {
            _eventHandler = eventHandler;
            _collectedEventArgs = new Dictionary<IScope, List<TEvent>>();
            _collectedEventArgsLock = new object();
        }

        public void HandleEvent(object sender, ScopedEventArgs<TEvent> eventArgs)
        {
            lock (_collectedEventArgsLock)
            {
                if (!_collectedEventArgs.TryGetValue(eventArgs.Scope, out var existingEventArgs))
                {
                    _collectedEventArgs.Add(eventArgs.Scope, new List<TEvent> { eventArgs.Value });
                    eventArgs.Scope.Stopped += ScopeStoppedHandler;
                }
                else
                {
                    existingEventArgs.Add(eventArgs.Value);
                }
            }
        }

        private void ScopeStoppedHandler(object sender, ScopeStoppedEventArgs eventArgs)
        {
            var collectedEventArgs = new List<TEvent>();

            lock (_collectedEventArgsLock)
            {
                eventArgs.Scope.Stopped -= ScopeStoppedHandler;

                if (!_collectedEventArgs.TryGetValue(eventArgs.Scope, out collectedEventArgs))
                    return;

                _collectedEventArgs.Remove(eventArgs.Scope);
            }

            _eventHandler.HandleEvent(this, new ScopedEventArgs<TEvent[]>(eventArgs.Scope, collectedEventArgs.ToArray()));
        }
    }
}
