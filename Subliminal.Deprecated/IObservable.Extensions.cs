using System;
using System.Reactive;

namespace Subliminal
{
    internal static class IObservableExtensions
    {
        public static ILog<TLogEntry> AsLog<TLogEntry>(this IObservable<TLogEntry> observable)
            where TLogEntry : ILogEntry
        {
            return DerivedLog<TLogEntry>.FromObservable(observable);
        }

        public static IEvent<TEvent> AsEvent<TEvent>(this IObservable<TEvent> observable)
        {
            return DerivedEvent<TEvent>.FromObservable(observable);
        }

        public static IEvent AsEvent(this IObservable<Unit> observable)
        {
            return DerivedEvent.FromObservable(observable);
        }

        public static IEventLog<TEvent> AsEventLog<TEvent>(this IObservable<TEvent> observable)
        {
            return DerivedEventLog<TEvent>.FromObservable(observable);
        }

        public static IEventLog AsEventLog(this IObservable<Unit> observable)
        {
            return DerivedEventLog.FromObservable(observable);
        }

        public static ICounter AsCounter(this IObservable<Count> observable)
        {
            return new DerivedCounter(observable.AsLog());
        }

        public static IGauge AsGauge(this IObservable<double> observable)
        {
            return DerivedGauge.FromObservable(observable);
        }
    }
}
