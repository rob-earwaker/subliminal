using System;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Event<TEvent> : IEvent<TEvent>
    {
        private readonly Subject<TEvent> _eventSubject;
        private readonly IEvent<TEvent> _derivedEvent;

        public Event()
        {
            _eventSubject = new Subject<TEvent>();
            _derivedEvent = _eventSubject.AsEvent();
        }

        public void Raise(TEvent @event)
        {
            _eventSubject.OnNext(@event);
            _eventSubject.OnCompleted();
        }

        public Guid EventId => _derivedEvent.EventId;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _derivedEvent.Subscribe(observer);
        }
    }
}
