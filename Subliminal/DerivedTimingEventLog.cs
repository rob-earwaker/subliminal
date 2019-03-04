using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedTimingEventLog<TTimingEvent> : ITimingEventLog<TTimingEvent>
        where TTimingEvent : ITiming
    {
        private readonly IEventLog<TTimingEvent> _eventLog;

        private DerivedTimingEventLog(IEventLog<TTimingEvent> eventLog)
        {
            _eventLog = eventLog;
        }

        public static DerivedTimingEventLog<TTimingEvent> FromObservable(IObservable<TTimingEvent> observable)
        {
            return new DerivedTimingEventLog<TTimingEvent>(observable.AsEventLog());
        }

        public ITimer Timer => _eventLog.Select(@event => @event.Duration).AsTimer();

        public ICounter Counter => _eventLog.Counter;

        public IDisposable Subscribe(IObserver<TTimingEvent> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
