using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Event<TEvent> : IEvent<TEvent>
    {
        private readonly ISubject<TEvent> _eventSubject;
        private readonly IEvent<TEvent> _derivedEvent;

        public Event()
        {
            // Synchronize the subject to ensure the event is only raised once
            // and that all subscribers receive the same value.
            _eventSubject = Subject.Synchronize(new AsyncSubject<TEvent>());

            _derivedEvent = _eventSubject.AsObservable().AsEvent();
        }

        public void Raise(TEvent @event)
        {
            _eventSubject.OnNext(@event);
            _eventSubject.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _derivedEvent.Subscribe(observer);
        }
    }
}
