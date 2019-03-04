using System;

namespace Subliminal
{
    public class TimingEvent<TTimingEvent> : ITimingEvent<TTimingEvent>
        where TTimingEvent : ITiming
    {
        private readonly TimingEventLog<TTimingEvent> _timingEventlog;

        public TimingEvent()
        {
            _timingEventlog = new TimingEventLog<TTimingEvent>();
        }

        public void LogAndClose(TTimingEvent @event)
        {
            _timingEventlog.Log(@event);
            _timingEventlog.Close();
        }

        public ITimer Timer => _timingEventlog.Timer;

        public ICounter Counter => _timingEventlog.Counter;

        public IDisposable Subscribe(IObserver<TTimingEvent> observer)
        {
            return _timingEventlog.Subscribe(observer);
        }
    }
}
