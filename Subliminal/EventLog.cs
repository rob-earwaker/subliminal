using System;
using System.Reactive;

namespace Subliminal
{
    public class EventLog : IEventLog
    {
        private readonly Log<Unit> _eventLog;
        private readonly IEventLog _derivedEventLog;

        public EventLog()
        {
            _eventLog = new Log<Unit>();
            _derivedEventLog = _eventLog.AsEventLog();
        }

        public Guid EventLogId => _derivedEventLog.EventLogId;
        public IObservable<Event> EventLogged => _derivedEventLog.EventLogged;
        public ICounter EventCounter => _derivedEventLog.EventCounter;

        public void LogOccurrence()
        {
            _eventLog.Append(Unit.Default);
        }
    }

    public class EventLog<TContext> : IEventLog<TContext>
    {
        private readonly Log<TContext> _eventLog;
        private readonly IEventLog<TContext> _derivedEventLog;

        public EventLog()
        {
            _eventLog = new Log<TContext>();
            _derivedEventLog = _eventLog.AsEventLog();
        }

        public Guid EventLogId => _derivedEventLog.EventLogId;
        public IObservable<Event<TContext>> EventLogged => _derivedEventLog.EventLogged;
        public ICounter EventCounter => _derivedEventLog.EventCounter;
        
        public void LogOccurrence(TContext context)
        {
            _eventLog.Append(context);
        }
    }
}
