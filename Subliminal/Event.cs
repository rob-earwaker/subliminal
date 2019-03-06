using System;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Event<TEvent> : IEvent<TEvent>
    {
        private readonly Subject<TEvent> _eventSubject;
        private readonly IEvent<TEvent> _event;

        public Event()
        {
            _eventSubject = new Subject<TEvent>();
            _event = _eventSubject.AsEvent();
        }

        public void Raise(TEvent @event)
        {
            _eventSubject.OnNext(@event);
            _eventSubject.OnCompleted();
        }

        public Guid EventId => _event.EventId;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _event.Subscribe(observer);
        }
    }
}
