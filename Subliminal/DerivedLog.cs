using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedLog<TValue> : ILog<TValue>
    {
        private readonly IObservable<TValue> _log;

        private DerivedLog(IObservable<TValue> log)
        {
            _log = log;
        }

        public static DerivedLog<TValue> FromObservable(IObservable<TValue> observable)
        {
            return new DerivedLog<TValue>(observable.Publish().AutoConnect());
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
