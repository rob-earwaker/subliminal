using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IObservableExtensions
    {
        public static IMetric<TValue> AsMetric<TValue>(this IObservable<TValue> observable)
        {
            return DerivedMetric<TValue>.FromObservable(observable);
        }

        public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, Operation operation)
        {
            return source.Buffer(operation.Started, operationStarted => operationStarted.Ended);
        }
    }
}
