using System;
using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// A log that is derived from an observable source.
    /// </summary>
    public sealed class DerivedLog<TEntry> : ILog<TEntry>
    {
        private readonly IObservable<TEntry> _entryLogged;

        private DerivedLog(IObservable<TEntry> entryLogged)
        {
            _entryLogged = entryLogged;
        }

        /// <summary>
        /// Creates a log from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static DerivedLog<TEntry> FromObservable(IObservable<TEntry> observable)
        {
            // Publish the observable to ensure that all observers receive
            // the same entries.
            var entryLogged = observable.Publish();

            // Connect to the published observable to start emitting entries
            // from the underlying source immediately.
            entryLogged.Connect();

            return new DerivedLog<TEntry>(entryLogged);
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future entries emitted
        /// by the log. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TEntry> observer)
        {
            return _entryLogged.Subscribe(observer);
        }
    }
}
