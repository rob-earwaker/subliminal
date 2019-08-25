using System.Reactive;

namespace Subliminal
{
    public interface IEventLog<out TEvent> : ILog<TEvent>
    {
        ICounter<long> EventCounter { get; }
    }

    public interface IEventLog : IEventLog<Unit>
    {
    }
}
