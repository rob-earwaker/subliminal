using System.Reactive;

namespace Subliminal
{
    public interface IEventLog<TEvent> : ILog<TEvent>
    {
    }

    public interface IEventLog : IEventLog<Unit>
    {
    }
}
