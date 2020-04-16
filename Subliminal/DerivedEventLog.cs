using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// An event log that is derived from an observable source.
    /// </summary>
    public sealed class DerivedEventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly ILog<TEvent> _eventLog;

        private DerivedEventLog(ILog<TEvent> eventLog)
        {
            _eventLog = eventLog;
        }

        /// <summary>
        /// A counter that increments every time an event is raised.
        /// </summary>
        public ICounter EventCounter => _eventLog.Select(_ => 1.0).AsCounter();

        /// <summary>
        /// Creates an event log from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static DerivedEventLog<TEvent> FromObservable(IObservable<TEvent> observable)
        {
            return new DerivedEventLog<TEvent>(observable.AsLog());
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future events emitted
        /// by the event log. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }

    /// <summary>
    /// An event log that is derived from an observable source.
    /// </summary>
    public sealed class DerivedEventLog : IEventLog
    {
        private readonly IEventLog<Unit> _eventLog;

        private DerivedEventLog(IEventLog<Unit> eventLog)
        {
            _eventLog = eventLog;
        }

        /// <summary>
        /// A counter that increments every time an event is raised.
        /// </summary>
        public ICounter EventCounter => _eventLog.EventCounter;

        /// <summary>
        /// Creates an event log from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static DerivedEventLog FromObservable(IObservable<Unit> observable)
        {
            return new DerivedEventLog(observable.AsEventLog<Unit>());
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future events emitted
        /// by the event log. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
