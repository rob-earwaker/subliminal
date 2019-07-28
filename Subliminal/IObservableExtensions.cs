using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IObservableExtensions
    {
        public static ILog<TValue> AsLog<TValue>(this IObservable<TValue> values)
        {
            return DerivedLog<TValue>.FromValues(values);
        }

        public static IGauge<TValue> AsGauge<TValue>(this IObservable<TValue> values)
        {
            return DerivedGauge<TValue>.FromObservable(values);
        }

        public static IEventLog<TContext> AsEventLog<TContext>(this IObservable<TContext> context)
        {
            return DerivedEventLog<TContext>.FromObservable(context);
        }

        public static ITrigger<TEvent> AsEvent<TEvent>(this IObservable<TEvent> observable)
        {
            return DerivedTrigger<TEvent>.FromObservable(observable);
        }

        public static ICounter AsCounter(this IObservable<long> increments)
        {
            return DerivedCounter.FromIncrements(increments);
        }

        public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, IOperation operation)
        {
            return source.Buffer(operation.Started.Events, operationStarted => operationStarted.Ended);
        }
    }
}
