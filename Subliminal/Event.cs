using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Event<TContext> : IEvent<TContext>
    {
        private readonly ISubject<TContext> _eventSubject;
        private readonly IEvent<TContext> _derivedEvent;

        public Event()
        {
            // Synchronize the subject to ensure the event is only raised once
            // and that all subscribers receive the same context value.
            _eventSubject = Subject.Synchronize(new AsyncSubject<TContext>());

            _derivedEvent = _eventSubject.AsObservable().AsEvent();
        }

        public void Raise(TContext context)
        {
            _eventSubject.OnNext(context);
            _eventSubject.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<TContext> observer)
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
