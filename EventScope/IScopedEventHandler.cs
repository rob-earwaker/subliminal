namespace EventScope
{
    public interface IScopedEventHandler<TEventArgs>
    {
        void HandleEvent(object sender, ScopedEventArgs<TEventArgs> eventArgs);
    }
}
