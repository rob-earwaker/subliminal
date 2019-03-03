using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IObservableExtensions
    {
        public static ILog<TValue> AsLog<TValue>(this IObservable<TValue> observable)
        {
            return DerivedLog<TValue>.FromObservable(observable);
        }

        public static IMetric<TValue> AsMetric<TValue>(this IObservable<TValue> observable)
        {
            return DerivedMetric<TValue>.FromObservable(observable);
        }

        public static IEventLog<TEvent> AsEventLog<TEvent>(this IObservable<TEvent> observable)
        {
            return DerivedEventLog<TEvent>.FromObservable(observable);
        }

        public static IEvent<TEvent> AsEvent<TEvent>(this IObservable<TEvent> observable)
        {
            return DerivedEvent<TEvent>.FromObservable(observable);
        }

        public static ICounter AsCounter(this IObservable<int> observable)
        {
            return DerivedCounter.FromObservable(observable);
        }

        public static ITimer AsTimer(this IObservable<TimeSpan> observable)
        {
            return DerivedTimer.FromObservable(observable);
        }

        public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, IOperation operation)
        {
            return source.Buffer(operation.Started, operationStarted => operationStarted.Ended);
        }
    }
}
