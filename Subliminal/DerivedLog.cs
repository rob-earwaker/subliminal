using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedLog<TEntry> : ILog<TEntry>
    {
        private readonly IObservable<TEntry> _entries;

        private DerivedLog(IObservable<TEntry> entries)
        {
            _entries = entries;
            LogId = Guid.NewGuid();
        }

        public Guid LogId { get; }

        public static DerivedLog<TEntry> FromObservable(IObservable<TEntry> observable)
        {
            return new DerivedLog<TEntry>(observable.Publish().AutoConnect());
        }

        public IDisposable Subscribe(IObserver<TEntry> observer)
        {
            return _entries.Subscribe(observer);
        }
    }
}
