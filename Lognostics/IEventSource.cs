using System;

namespace Lognostics
{
    public interface IEventSource<TEventArgs> where TEventArgs : EventArgs
    {
        void AddHandler(IEventHandler<TEventArgs> eventHandler);
        void AddHandler(EventHandler<TEventArgs> eventHandler);
        void RemoveHandler(IEventHandler<TEventArgs> eventHandler);
        void RemoveHandler(EventHandler<TEventArgs> eventHandler);
    }
}
