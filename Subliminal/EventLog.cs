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
            _derivedEventLog = _eventLog.AsEventLog();
        }

        public void Log(TEvent @event)
        {
            _eventLog.Append(@event);
        }

        public Guid EventLogId => _derivedEventLog.EventLogId;

        public ICounter EventCounter => _derivedEventLog.EventCounter;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _derivedEventLog.Subscribe(observer);
        }
    }
}
