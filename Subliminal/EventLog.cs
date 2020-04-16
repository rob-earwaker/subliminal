using System;
using System.Reactive;

namespace Subliminal
{
    /// <summary>
    /// An event log that both captures and emits events.
    /// </summary>
    public sealed class EventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly Log<TEvent> _eventLog;
        private readonly IEventLog<TEvent> _derivedEventLog;

        /// <summary>
        /// Creates an event log that both captures and emits events.
        /// </summary>
        public EventLog()
        {
            _eventLog = new Log<TEvent>();
            _derivedEventLog = _eventLog.AsEventLog();
        }

        /// <summary>
        /// A counter that increments every time an event is raised.
        /// </summary>
        public ICounter EventCounter => _derivedEventLog.EventCounter;

        /// <summary>
        /// Captures an event and emits it to all observers.
        /// </summary>
        public void LogOccurrence(TEvent @event)
        {
            _eventLog.Append(@event);
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future events emitted
        /// by the event log. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _derivedEventLog.Subscribe(observer);
        }
    }

    /// <summary>
    /// An event log that both captures and emits events.
    /// </summary>
    public sealed class EventLog : IEventLog
    {
        private readonly EventLog<Unit> _eventLog;

        /// <summary>
        /// Creates an event log that both captures and emits events.
        /// </summary>
        public EventLog()
        {
            _eventLog = new EventLog<Unit>();
        }

        /// <summary>
        /// A counter that increments every time an event is raised.
        /// </summary>
        public ICounter EventCounter => _eventLog.EventCounter;

        /// <summary>
        /// Captures an event and emits it to all observers.
        /// </summary>
        public void LogOccurrence()
        {
            _eventLog.LogOccurrence(Unit.Default);
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future events emitted
        /// by the event log. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
