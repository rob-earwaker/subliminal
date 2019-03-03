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

        public static ICounter AsCounter(this IObservable<int> observable)
        {
            return DerivedCounter.FromObservable(observable);
        }

        public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, IOperationLog operationLog)
        {
            return source.Buffer(operationLog.OperationStarted, operationStarted => operationStarted.EndedEvent);
        }
    }
}
