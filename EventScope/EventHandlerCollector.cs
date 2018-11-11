using System.Collections.Generic;

namespace EventScope
{
    public class EventHandlerCollector<TEventArgs> : IEventSource<TEventArgs> where TEventArgs : ScopedEventArgs
    {
        private readonly HashSet<IEventHandler<TEventArgs>> _eventHandlers;
        private readonly object _eventHandlersLock;

        public EventHandlerCollector()
        {
            _eventHandlers = new HashSet<IEventHandler<TEventArgs>>();
            _eventHandlersLock = new object();
        }

        public void AddHandler(IEventHandler<TEventArgs> eventHandler)
        {
            lock (_eventHandlersLock)
            {
                _eventHandlers.Add(eventHandler);
            }
        }

        public void RemoveHandler(IEventHandler<TEventArgs> eventHandler)
        {
            throw new System.NotImplementedException();
        }
    }
}
