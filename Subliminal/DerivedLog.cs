using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedLog<TEntry> : ILog<TEntry>
    {
        private readonly IObservable<TEntry> _entryLogged;

        private DerivedLog(IObservable<TEntry> entryLogged)
        {
            _entryLogged = entryLogged;
        }

        public static DerivedLog<TEntry> FromObservable(IObservable<TEntry> observable)
        {
            // Publish the observable to ensure that all future subscribers to
            // the log receive the same entries.
            var entryLogged = observable.Publish();

            // Connect to the published observable to start emitting entries
            // from the underlying source immediately.
            entryLogged.Connect();

            return new DerivedLog<TEntry>(entryLogged);
        }

        public IDisposable Subscribe(IObserver<TEntry> observer)
        {
            return _entryLogged.Subscribe(observer);
        }
    }
}
