using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    /// <summary>
    /// An event that both captures and emits a single value.
    /// </summary>
    public sealed class Event<TEvent> : IEvent<TEvent>
    {
        private readonly ISubject<TEvent> _eventSubject;
        private readonly IEvent<TEvent> _derivedEvent;

        /// <summary>
        /// Creates an event that both captures and emits a single value.
        /// </summary>
        public Event()
        {
            // Synchronize the subject to ensure that multiple events
            // are not raised at the same time and therefore that all
            // subscribers receive the same event.
            _eventSubject = Subject.Synchronize(new Subject<TEvent>());

            _derivedEvent = _eventSubject.AsObservable().AsEvent();
        }

        /// <summary>
        /// Captures the event and emits it to all observers.
        /// </summary>
        public void Raise(TEvent @event)
        {
            _eventSubject.OnNext(@event);
            _eventSubject.OnCompleted();
        }

        /// <summary>
        /// Subscribes an observer such that it receives the event when it is
        /// raised, or immediately if it has already been raised. The returned
        /// <see cref="IDisposable" /> can be used to cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _derivedEvent.Subscribe(observer);
        }
    }

    /// <summary>
    /// An event that both captures and emits a single value.
    /// </summary>
    public sealed class Event : IEvent
    {
        private readonly Event<Unit> _event;

        /// <summary>
        /// Creates an event that both captures and emits a single value.
        /// </summary>
        public Event()
        {
            _event = new Event<Unit>();
        }

        /// <summary>
        /// Captures the event and emits it to all observers.
        /// </summary>
        public void Raise()
        {
            _event.Raise(Unit.Default);
        }

        /// <summary>
        /// Subscribes an observer such that it receives the event when it is
        /// raised, or immediately if it has already been raised. The returned
        /// <see cref="IDisposable" /> can be used to cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return _event.Subscribe(observer);
        }
    }
}
