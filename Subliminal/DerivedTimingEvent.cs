using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedTimingEvent<TTimingEvent> : ITimingEvent<TTimingEvent>
        where TTimingEvent : ITiming
    {
        private readonly ITimingEventLog<TTimingEvent> _timingEventLog;

        private DerivedTimingEvent(ITimingEventLog<TTimingEvent> timingEventLog)
        {
            _timingEventLog = timingEventLog;
        }

        public static DerivedTimingEvent<TTimingEvent> FromObservable(IObservable<TTimingEvent> observable)
        {
            return new DerivedTimingEvent<TTimingEvent>(observable.Take(1).AsTimingEventLog());
        }

        public ITimer Timer => _timingEventLog.Timer;

        public ICounter Counter => _timingEventLog.Counter;

        public IDisposable Subscribe(IObserver<TTimingEvent> observer)
        {
            return _timingEventLog.Subscribe(observer);
        }
    }
}
