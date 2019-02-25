using Subliminal.Events;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IObservableExtensions
    {
        public static IGauge<TValue> AsGauge<TValue>(this IObservable<TValue> source)
        {
            return new Gauge<TValue>(source
                .Timestamp()
                .TimeInterval()
                .Select(x => new GaugeSampled<TValue>(x.Value.Value, x.Value.Timestamp, x.Interval)));
        }

        public static IGauge<TValue> AsGauge<TValue>(this IObservable<GaugeSampled<TValue>> sampled)
        {
            return new Gauge<TValue>(sampled);
        }

        public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, Operation operation)
        {
            return source.Buffer(operation.Started, started => started.Operation.Ended);
        }
    }
}
