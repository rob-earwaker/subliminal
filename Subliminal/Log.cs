using System;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Log<TValue> : ILog<TValue>
    {
        private readonly Subject<TValue> _log;

        public Log()
        {
            _log = new Subject<TValue>();
        }

        public void Append(TValue value)
        {
            _log.OnNext(value);
        }

        public void Close()
        {
            _log.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
