using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IObservableExtensions
    {
        public static ISource<TValue> AsSource<TValue>(this IObservable<TValue> observable)
        {
            return Source<TValue>.FromObservable(observable);
        }

        public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, Operation operation)
        {
            return source.Buffer(operation.Started, started => started.Operation.Ended);
        }
    }
}
