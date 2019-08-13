using System;
using System.Reactive;

namespace Subliminal
{
    public class EventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly Log<TEvent> _eventLog;
        private readonly IEventLog<TEvent> _derivedEventLog;

        public EventLog()
        {
            _eventLog = new Log<TEvent>();
            _derivedEventLog = _eventLog.AsEventLog();
        }

        public void LogOccurrence(TEvent @event)
        {
            _eventLog.Append(@event);
        }

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _derivedEventLog.Subscribe(observer);
        }
    }

    public class EventLog : IEventLog
    {
        private readonly EventLog<Unit> _eventLog;

        public EventLog()
        {
            _eventLog = new EventLog<Unit>();
        }

        public void LogOccurrence()
        {
            _eventLog.LogOccurrence(Unit.Default);
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
