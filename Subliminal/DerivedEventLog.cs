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

        public static DerivedEventLog<TEvent> FromObservable(IObservable<TEvent> observable)
        {
            return new DerivedEventLog<TEvent>(observable.AsLog());
        }

        public Guid EventLogId => _eventLog.LogId;

        public ICounter EventCounter => _eventLog.Select(_ => 1L).AsCounter();

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
