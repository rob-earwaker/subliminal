using System;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Log<TEntry> : ILog<TEntry>
    {
        private readonly Subject<TEntry> _log;

        public Log()
        {
            _log = new Subject<TEntry>();
        }

        public void Append(TEntry value)
        {
            _log.OnNext(value);
        }

        public void Close()
        {
            _log.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<TEntry> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
