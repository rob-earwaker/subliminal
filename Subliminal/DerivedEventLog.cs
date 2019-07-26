using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedEventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly ILog<TEvent> _eventLog;

        private DerivedEventLog(ILog<TEvent> eventLog)
        {
            _eventLog = eventLog;
        }

        public static DerivedEventLog<TEvent> FromObservable(IObservable<TEvent> events)
        {
            return new DerivedEventLog<TEvent>(events.AsLog());
        }

        public Guid EventLogId => _eventLog.LogId;
        public IObservable<TEvent> Events => _eventLog.Entries;
        public ICounter EventCounter => _eventLog.Entries.Select(_ => 1L).AsCounter();
    }
}
