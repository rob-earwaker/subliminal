using System;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Log<TEntry> : ILog<TEntry>
    {
        private readonly Subject<TEntry> _logSubject;
        private readonly ILog<TEntry> _log;

        public Log()
        {
            _logSubject = new Subject<TEntry>();
            _log = _logSubject.AsLog();
        }

        public void Append(TEntry value)
        {
            _logSubject.OnNext(value);
        }

        public void Close()
        {
            _logSubject.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<TEntry> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
