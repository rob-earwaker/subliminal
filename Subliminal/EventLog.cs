using System;

namespace Subliminal
{
    public class EventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly Log<TEvent> _log;
        private readonly IEventLog<TEvent> _eventLog;

        public EventLog()
        {
            _log = new Log<TEvent>();
            _eventLog = _log.AsEventLog();
        }

        public void Log(TEvent @event)
        {
            _log.Append(@event);
        }

        public Guid EventLogId => _eventLog.EventLogId;

        public ICounter EventCounter => _eventLog.EventCounter;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
