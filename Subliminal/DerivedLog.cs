using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedLog<TEntry> : ILog<TEntry>
    {
        private readonly IObservable<TEntry> _log;

        private DerivedLog(IObservable<TEntry> log)
        {
            _log = log;
        }

        public static DerivedLog<TEntry> FromObservable(IObservable<TEntry> observable)
        {
            return new DerivedLog<TEntry>(observable.Publish().AutoConnect());
        }

        public IDisposable Subscribe(IObserver<TEntry> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
