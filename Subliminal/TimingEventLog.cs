using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class TimingEventLog<TTimingEvent> : ITimingEventLog<TTimingEvent>
        where TTimingEvent : ITiming
    {
        private readonly EventLog<TTimingEvent> _eventLog;

        public TimingEventLog()
        {
            _eventLog = new EventLog<TTimingEvent>();
        }

        public void Log(TTimingEvent @event)
        {
            _eventLog.Log(@event);
        }

        public void Close()
        {
            _eventLog.Close();
        }

        public ITimer Timer => _eventLog.Select(@event => @event.Duration).AsTimer();

        public ICounter Counter => _eventLog.Counter;

        public IDisposable Subscribe(IObserver<TTimingEvent> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
