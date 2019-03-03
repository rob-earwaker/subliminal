using System;

namespace Subliminal
{
    public class EventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly Log<TEvent> _log;
        private readonly Counter _counter;

        public EventLog()
        {
            _log = new Log<TEvent>();
            _counter = new Counter();
        }

        public void Log(TEvent @event)
        {
            _log.Append(@event);
            _counter.Increment();
        }

        public void Close()
        {
            _log.Close();
            _counter.Stop();
        }

        public ICounter Counter => _counter;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
