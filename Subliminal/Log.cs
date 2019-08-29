using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    /// <summary>
    /// A log that both captures and emits entries.
    /// </summary>
    public sealed class Log<TEntry> : ILog<TEntry>
    {
        private readonly ISubject<TEntry> _logSubject;
        private readonly ILog<TEntry> _derivedLog;

        /// <summary>
        /// Creates a log that both captures and emits entries.
        /// </summary>
        public Log()
        {
            // Synchronize the subject to ensure that multiple entries
            // are not logged at the same time and therefore that all
            // subscribers receive entries in the same order.
            _logSubject = Subject.Synchronize(new Subject<TEntry>());

            _derivedLog = _logSubject.AsObservable().AsLog();
        }

        /// <summary>
        /// Captures an entry and emits it to all observers.
        /// </summary>
        public void Append(TEntry entry)
        {
            _logSubject.OnNext(entry);
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future entries emitted
        /// by the log. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TEntry> observer)
        {
            return _derivedLog.Subscribe(observer);
        }
    }
}
