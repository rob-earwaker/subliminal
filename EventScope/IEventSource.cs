namespace EventScope
{
    public interface IEventSource<TEventArgs> where TEventArgs : ScopedEventArgs
    {
        void AddHandler(IEventHandler<TEventArgs> eventHandler);
        void RemoveHandler(IEventHandler<TEventArgs> eventHandler);
    }
}
