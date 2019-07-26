using System;

namespace Subliminal
{
    public class EventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly Log<TEvent> _eventLog;
        private readonly IEventLog<TEvent> _derivedEventLog;

        public EventLog()
        {
            _eventLog = new Log<TEvent>();
            _derivedEventLog = _eventLog.Entries.AsEventLog();
        }

        public Guid EventLogId => _derivedEventLog.EventLogId;
        public IObservable<TEvent> Events => _derivedEventLog.Events;
        public ICounter EventCounter => _derivedEventLog.EventCounter;

        public void Log(TEvent @event)
        {
            _eventLog.Append(@event);
        }
    }
}
