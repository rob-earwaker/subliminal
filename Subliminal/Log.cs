using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Log<TValue> : ILog<TValue>
    {
        private readonly Subject<TValue> _logSubject;
        private readonly ILog<TValue> _derivedLog;

        public Log()
        {
            _logSubject = new Subject<TValue>();
            _derivedLog = _logSubject.AsObservable().AsLog();
        }

        public Guid LogId => _derivedLog.LogId;
        public IObservable<LogEntry<TValue>> EntryLogged => _derivedLog.EntryLogged;

        public void Append(TValue value)
        {
            _logSubject.OnNext(value);
        }
    }
}
