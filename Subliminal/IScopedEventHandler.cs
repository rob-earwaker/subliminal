using Subliminal.Events;

namespace Subliminal
{
    public interface IScopedEventHandler<TValue> : IEventHandler<Scoped<TValue>>
    {
    }
}
