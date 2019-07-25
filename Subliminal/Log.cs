using System;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Log<TEntry> : ILog<TEntry>
    {
        private readonly Subject<TEntry> _logSubject;
        private readonly ILog<TEntry> _derivedLog;

        public Log()
        {
            _logSubject = new Subject<TEntry>();
            _derivedLog = _logSubject.AsLog();
        }

        public Guid LogId => _derivedLog.LogId;

        public IObservable<TEntry> Entries => _derivedLog.Entries;

        public void Append(TEntry value)
        {
            _logSubject.OnNext(value);
        }
    }
}
