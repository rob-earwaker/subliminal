using System;

namespace Subliminal
{
    public class AggregateEventHandler<TEventArgs> : IEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly IEventHandler<TEventArgs>[] _eventHandlers;

        public AggregateEventHandler(params IEventHandler<TEventArgs>[] eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        public void HandleEvent(object sender, TEventArgs eventArgs)
        {
            foreach (var eventHandler in _eventHandlers)
            {
                eventHandler?.HandleEvent(sender, eventArgs);
            }
        }
    }
}
