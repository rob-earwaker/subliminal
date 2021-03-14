using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public sealed class Log<TLogEntry> : ILog<TLogEntry> where TLogEntry : ILogEntry
    {
        private readonly ISubject<TLogEntry> _logSubject;

        public Log()
        {
            // Synchronize the subject to ensure that multiple entries
            // are not logged at the same time and therefore that all
            // subscribers receive entries in the same order.
            _logSubject = Subject.Synchronize(new Subject<TLogEntry>());
        }

        public void LogEntry(TLogEntry entry)
        {
            _logSubject.OnNext(entry);
        }

        public IDisposable Subscribe(ILogHandler<TLogEntry> logHandler)
        {
            return _logSubject.Subscribe(logHandler.Handle);
        }

        private static ILog<TNewLogEntry> Transform<TNewLogEntry>(
            ILog<TLogEntry> log, Func<IObservable<TLogEntry>, IObservable<TNewLogEntry>> transform)
            where TNewLogEntry : ILogEntry
        {
            return DerivedLog<TNewLogEntry>.FromObservable(transform(log._logSubject));
        }
    }
}
