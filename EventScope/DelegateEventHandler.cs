using System;

namespace EventScope
{
    public class DelegateEventHandler<TEventArgs> : IEventHandler<TEventArgs> where TEventArgs : ScopedEventArgs
    {
        private readonly Action<object, TEventArgs> _handleEvent;

        public DelegateEventHandler(Action<object, TEventArgs> handleEvent)
        {
            _handleEvent = handleEvent;
        }

        public void HandleEvent(object sender, TEventArgs eventArgs)
        {
            _handleEvent?.Invoke(sender, eventArgs);
        }
    }
}
