using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public sealed class DerivedLog<TLogEntry> : ILog<TLogEntry> where TLogEntry : ILogEntry
    {
        private readonly IObservable<TLogEntry> _entryLogged;

        private DerivedLog(IObservable<TLogEntry> entryLogged)
        {
            _entryLogged = entryLogged;
        }

        public static DerivedLog<TLogEntry> FromObservable(IObservable<TLogEntry> observable)
        {
            // Publish the observable to ensure that all observers receive
            // the same entries.
            var entryLogged = observable.Publish();

            // Connect to the published observable to start emitting entries
            // from the underlying source immediately.
            entryLogged.Connect();

            return new DerivedLog<TLogEntry>(entryLogged);
        }

        public IDisposable Subscribe(ILogHandler<TLogEntry> logHandler)
        {
            return _entryLogged.Subscribe(logHandler.Handle);
        }
    }
}
