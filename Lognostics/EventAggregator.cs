using Lognostics.Events;
using System.Collections.Generic;

namespace Lognostics
{
    public static class EventAggregator
    {
        public static EventAggregator<TValue> Create<TValue>(IScopedEventHandler<TValue[]> eventHandler)
        {
            return new EventAggregator<TValue>(eventHandler);
        }
    }

    public class EventAggregator<TValue> : IScopedEventHandler<TValue>
    {
        private readonly IScopedEventHandler<TValue[]> _eventHandler;
        private readonly Dictionary<IScope, List<TValue>> _collectedEventArgs;
        private readonly object _collectedEventArgsLock;

        public EventAggregator(IScopedEventHandler<TValue[]> eventHandler)
        {
            _eventHandler = eventHandler;
            _collectedEventArgs = new Dictionary<IScope, List<TValue>>();
            _collectedEventArgsLock = new object();
        }

        public void HandleEvent(object sender, Scoped<TValue> eventArgs)
        {
            lock (_collectedEventArgsLock)
            {
                if (!_collectedEventArgs.TryGetValue(eventArgs.Scope, out var existingEventArgs))
                {
                    _collectedEventArgs.Add(eventArgs.Scope, new List<TValue> { eventArgs.Value });
                    eventArgs.Scope.Ended += ScopeEndedHandler;
                }
                else
                {
                    existingEventArgs.Add(eventArgs.Value);
                }
            }
        }

        private void ScopeEndedHandler(object sender, ScopeEnded eventArgs)
        {
            var collectedEventArgs = new List<TValue>();

            lock (_collectedEventArgsLock)
            {
                eventArgs.Scope.Ended -= ScopeEndedHandler;

                if (!_collectedEventArgs.TryGetValue(eventArgs.Scope, out collectedEventArgs))
                    return;

                _collectedEventArgs.Remove(eventArgs.Scope);
            }

            _eventHandler.HandleEvent(this, new Scoped<TValue[]>(eventArgs.Scope, collectedEventArgs.ToArray()));
        }
    }
}
