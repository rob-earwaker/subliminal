using System;
using System.Reactive.Linq;
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
            _derivedLog = _logSubject.AsObservable().AsLog();
        }

        public void Append(TEntry value)
        {
            _logSubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<TEntry> observer)
        {
            return _derivedLog.Subscribe(observer);
        }
    }
}
