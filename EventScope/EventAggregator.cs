using System;
using System.Collections.Generic;

namespace EventScope
{
    public class EventAggregator<TEventArgs> : IScopedEventHandler<TEventArgs>
    {
        private readonly Dictionary<IScope, List<TEventArgs>> _collectedEventArgs;
        private readonly object _collectedEventArgsLock;

        public EventAggregator()
        {
            _collectedEventArgs = new Dictionary<IScope, List<TEventArgs>>();
            _collectedEventArgsLock = new object();
        }

        public event EventHandler<ScopedEventArgs<TEventArgs[]>> EventsAggregated;

        public void HandleEvent(object sender, ScopedEventArgs<TEventArgs> eventArgs)
        {
            lock (_collectedEventArgsLock)
            {
                if (!_collectedEventArgs.TryGetValue(eventArgs.Scope, out var existingEventArgs))
                {
                    _collectedEventArgs.Add(eventArgs.Scope, new List<TEventArgs> { eventArgs.Value });
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
            var collectedEventArgs = new List<TEventArgs>();

            lock (_collectedEventArgsLock)
            {
                eventArgs.Scope.Stopped -= ScopeStoppedHandler;

                if (!_collectedEventArgs.TryGetValue(eventArgs.Scope, out collectedEventArgs))
                    return;

                _collectedEventArgs.Remove(eventArgs.Scope);
            }

            EventsAggregated?.Invoke(this, new ScopedEventArgs<TEventArgs[]>(eventArgs.Scope, collectedEventArgs.ToArray()));
        }
    }
}
