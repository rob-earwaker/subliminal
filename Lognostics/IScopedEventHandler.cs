namespace Lognostics
{
    public interface IScopedEventHandler<TValue> : IEventHandler<ScopedEventArgs<TValue>>
    {
    }
}
