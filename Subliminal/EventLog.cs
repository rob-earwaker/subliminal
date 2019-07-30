using System;
using System.Reactive;

namespace Subliminal
{
    public class EventLog<TContext> : IEventLog<TContext>
    {
        private readonly Log<TContext> _eventLog;
        private readonly IEventLog<TContext> _derivedEventLog;

        public EventLog()
        {
            _eventLog = new Log<TContext>();
            _derivedEventLog = _eventLog.AsEventLog();
        }

        public void LogOccurrence(TContext context)
        {
            _eventLog.Append(context);
        }

        public IDisposable Subscribe(IObserver<TContext> observer)
        {
            return _derivedEventLog.Subscribe(observer);
        }
    }

    public class EventLog : IEventLog<Unit>
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
