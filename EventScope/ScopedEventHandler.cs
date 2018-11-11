using System;

namespace EventScope
{
    public class ScopedEventHandler<TEventArgs> : IEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly IScopedEventHandler<TEventArgs> _eventHandler;
        private readonly IScopeSource _scopeSource;

        public ScopedEventHandler(IScopedEventHandler<TEventArgs> eventHandler, IScopeSource scopeSource)
        {
            _eventHandler = eventHandler;
            _scopeSource = scopeSource;
        }

        public void HandleEvent(object sender, TEventArgs eventArgs)
        {
            foreach (var activeScope in _scopeSource.ActiveScopes)
            {
                _eventHandler.HandleEvent(sender, new ScopedEventArgs<TEventArgs>(activeScope, eventArgs));
            }
        }
    }
}
