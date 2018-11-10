namespace EventScope
{
    public interface IEventHandler<TEventArgs> where TEventArgs : ScopedEventArgs
    {
        void HandleEvent(object sender, TEventArgs eventArgs);
    }
}
