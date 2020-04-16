using System;
using System.Reactive;

namespace Subliminal
{
    /// <summary>
    /// Contains extensions for the <see cref="IObservable{T}" /> interface.
    /// </summary>
    public static class IObservableExtensions
    {
        /// <summary>
        /// Creates a log from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static ILog<TEntry> AsLog<TEntry>(this IObservable<TEntry> observable)
        {
            return DerivedLog<TEntry>.FromObservable(observable);
        }

        /// <summary>
        /// Creates an event from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately. The
        /// event will be raised by the first item consumed.
        /// </summary>
        public static IEvent<TEvent> AsEvent<TEvent>(this IObservable<TEvent> observable)
        {
            return DerivedEvent<TEvent>.FromObservable(observable);
        }

        /// <summary>
        /// Creates an event from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately. The
        /// event will be raised by the first item consumed.
        /// </summary>
        public static IEvent AsEvent(this IObservable<Unit> observable)
        {
            return DerivedEvent.FromObservable(observable);
        }

        /// <summary>
        /// Creates an event log from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static IEventLog<TEvent> AsEventLog<TEvent>(this IObservable<TEvent> observable)
        {
            return DerivedEventLog<TEvent>.FromObservable(observable);
        }

        /// <summary>
        /// Creates an event log from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static IEventLog AsEventLog(this IObservable<Unit> observable)
        {
            return DerivedEventLog.FromObservable(observable);
        }

        /// <summary>
        /// Creates a counter from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static ICounter AsCounter(this IObservable<double> observable)
        {
            return DerivedCounter.FromObservable(observable);
        }

        /// <summary>
        /// Creates a gauge from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static IGauge AsGauge(this IObservable<double> observable)
        {
            return DerivedGauge.FromObservable(observable);
        }
    }
}
