using System;

namespace EventScope
{
    public class ManualEventSource<TEventArgs> : IEventSource<TEventArgs> where TEventArgs : ScopedEventArgs
    {
        private event EventHandler<TEventArgs> Event;
        
        public void AddHandler(IEventHandler<TEventArgs> eventHandler)
        {
            Event += eventHandler.HandleEvent;
        }

        public void RemoveHandler(IEventHandler<TEventArgs> eventHandler)
        {
            Event -= eventHandler.HandleEvent;
        }

        public void RaiseEvent(object sender, TEventArgs eventArgs)
        {
            Event?.Invoke(sender, eventArgs);
        }
    }
}