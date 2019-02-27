﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IObservableExtensions
    {
        internal static ISource<TValue> AsSource<TValue>(this IObservable<TValue> source)
        {
            return Source<TValue>.FromObservable(source);
        }

        public static IGauge<TValue> AsGauge<TValue>(this IObservable<TValue> source)
        {
            return Gauge<TValue>.FromSource(source);
        }

        public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, Operation operation)
        {
            return source.Buffer(operation.Started, started => started.Operation.Ended);
        }
    }
}
