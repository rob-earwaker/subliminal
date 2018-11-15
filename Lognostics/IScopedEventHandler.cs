using Lognostics.Events;

namespace Lognostics
{
    public interface IScopedEventHandler<TValue> : IEventHandler<Scoped<TValue>>
    {
    }
}
