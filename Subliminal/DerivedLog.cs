using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedLog<TValue> : ILog<TValue>
    {
        private DerivedLog(Guid logId, IObservable<LogEntry<TValue>> entrylogged)
        {
            LogId = logId;
            EntryLogged = entrylogged;
        }

        public static DerivedLog<TValue> FromLog(ILog<TValue> log)
        {
            var logId = Guid.NewGuid();

            var entryLogged = log.EntryLogged
                .Select(entry => new LogEntry<TValue>(logId, entry.Value, entry.Timestamp, entry.Interval));

            return new DerivedLog<TValue>(logId, entryLogged);
        }

        public static DerivedLog<TValue> FromValues(IObservable<TValue> values)
        {
            // Publish the observable to ensure that all future subscribers to
            // the log receive the same values.
            var publishedObservable = values.Publish();

            // Connect to the published observable to start emitting values
            // from the underlying source immediately.
            publishedObservable.Connect();

            var logId = Guid.NewGuid();

            var entryLogged = publishedObservable
                .Timestamp()
                .TimeInterval()
                .Select(value => new LogEntry<TValue>(logId, value.Value.Value, value.Value.Timestamp, value.Interval));

            return new DerivedLog<TValue>(logId, entryLogged);
        }

        public Guid LogId { get; }
        public IObservable<LogEntry<TValue>> EntryLogged { get; }
    }
}
