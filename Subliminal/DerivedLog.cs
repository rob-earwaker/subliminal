using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedLog<TEntry> : ILog<TEntry>
    {
        private DerivedLog(IObservable<TEntry> entries)
        {
            Entries = entries;
            LogId = Guid.NewGuid();
        }

        public static DerivedLog<TEntry> FromObservable(IObservable<TEntry> entries)
        {
            // Publish the observable to ensure that all future subscribers to
            // the log receive the same entries.
            var publishedEntries = entries.Publish();
            // Connect to the published observable to start emitting entries
            // from the underlying source immediately.
            publishedEntries.Connect();
            return new DerivedLog<TEntry>(publishedEntries);
        }

        public Guid LogId { get; }

        public IObservable<TEntry> Entries { get; }
    }
}
