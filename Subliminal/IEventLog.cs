using System.Reactive;

namespace Subliminal
{
    public interface IEventLog<TEvent> : ILog<TEvent>
    {
        ICounter<long> EventCounter { get; }
    }

    public interface IEventLog : IEventLog<Unit>
    {
    }
}
