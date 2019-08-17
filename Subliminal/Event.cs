using System;
using System.Reactive;
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
            // Synchronize the subject to ensure that multiple events
            // are not raised at the same time and therefore that all
            // subscribers receive the same event.
            _eventSubject = Subject.Synchronize(new Subject<TEvent>());

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

    public class Event : IEvent
    {
        private readonly Event<Unit> _event;

        public Event()
        {
            _event = new Event<Unit>();
        }

        public void Raise()
        {
            _event.Raise(Unit.Default);
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return _event.Subscribe(observer);
        }
    }
}
