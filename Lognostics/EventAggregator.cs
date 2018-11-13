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

        public void HandleEvent(object sender, ScopedEventArgs<TValue> eventArgs)
        {
            lock (_collectedEventArgsLock)
            {
                if (!_collectedEventArgs.TryGetValue(eventArgs.Scope, out var existingEventArgs))
                {
                    _collectedEventArgs.Add(eventArgs.Scope, new List<TValue> { eventArgs.Value });
                    eventArgs.Scope.Ended += ScopeStoppedHandler;
                }
                else
                {
                    existingEventArgs.Add(eventArgs.Value);
                }
            }
        }

        private void ScopeStoppedHandler(object sender, ScopeEndedEventArgs eventArgs)
        {
            var collectedEventArgs = new List<TValue>();

            lock (_collectedEventArgsLock)
            {
                eventArgs.Scope.Ended -= ScopeStoppedHandler;

                if (!_collectedEventArgs.TryGetValue(eventArgs.Scope, out collectedEventArgs))
                    return;

                _collectedEventArgs.Remove(eventArgs.Scope);
            }

            _eventHandler.HandleEvent(this, new ScopedEventArgs<TValue[]>(eventArgs.Scope, collectedEventArgs.ToArray()));
        }
    }
}
