using System.Reactive;

namespace Subliminal
{
    /// <summary>
    /// An observable log of events.
    /// </summary>
    public interface IEventLog<out TEvent> : ILog<TEvent>
    {
        /// <summary>
        /// A counter that increments every time an event is raised.
        /// </summary>
        ICounter<long> EventCounter { get; }
    }

    /// <summary>
    /// An observable log of events.
    /// </summary>
    public interface IEventLog : IEventLog<Unit>
    {
    }
}
