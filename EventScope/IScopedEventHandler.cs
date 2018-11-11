namespace EventScope
{
    public interface IScopedEventHandler<TValue> : IEventHandler<ScopedEventArgs<TValue>>
    {
    }
}
