using System;

namespace Subliminal
{
    public class Event<TEvent> : IEvent<TEvent>
    {
        private readonly Log<TEvent> _log;
        private readonly Counter _counter;

        public Event()
        {
            _log = new Log<TEvent>();
            _counter = new Counter();
        }

        public void Log(TEvent @event)
        {
            _log.Append(@event);
            _log.Close();
            _counter.Increment();
            _counter.Stop();
        }

        public ICounter Counter => _counter;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
