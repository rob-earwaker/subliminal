using System;

namespace Subliminal
{
    public class Event<TEvent> : IEvent<TEvent>
    {
        private readonly EventLog<TEvent> _eventLog;

        public Event()
        {
            _eventLog = new EventLog<TEvent>();
        }

        public void Log(TEvent @event)
        {
            _eventLog.Log(@event);
            _eventLog.Close();
        }

        public ICounter Counter => _eventLog.Counter;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
