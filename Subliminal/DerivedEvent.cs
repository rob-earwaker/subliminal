using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// An event that is derived from an observable source.
    /// </summary>
    public sealed class DerivedEvent<TEvent> : IEvent<TEvent>
    {
        private readonly IObservable<TEvent> _raised;

        private DerivedEvent(IObservable<TEvent> raised)
        {
            _raised = raised;
        }

        /// <summary>
        /// Creates an event from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately. The
        /// event will be raised by the first item consumed.
        /// </summary>
        public static DerivedEvent<TEvent> FromObservable(IObservable<TEvent> observable)
        {
            // Take a single value from the observable to ensure that the event
            // is only raised once, and replay this value to all observers.
            var raised = observable.Take(1).Replay();

            // Connect immediately so that observers can consume the event.
            raised.Connect();

            return new DerivedEvent<TEvent>(raised);
        }

        /// <summary>
        /// Subscribes an observer such that it receives the event when it is
        /// raised, or immediately if it has already been raised. The returned
        /// <see cref="IDisposable" /> can be used to cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _raised.Subscribe(observer);
        }
    }

    /// <summary>
    /// An event that is derived from an observable source.
    /// </summary>
    public sealed class DerivedEvent : IEvent
    {
        private readonly IEvent<Unit> _event;

        private DerivedEvent(IEvent<Unit> @event)
        {
            _event = @event;
        }

        /// <summary>
        /// Creates an event from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately. The
        /// event will be raised by the first item consumed.
        /// </summary>
        public static DerivedEvent FromObservable(IObservable<Unit> observable)
        {
            return new DerivedEvent(observable.AsEvent<Unit>());
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
