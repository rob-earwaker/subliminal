using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IObservableExtensions
    {
        public static ILog<TEntry> AsLog<TEntry>(this IObservable<TEntry> observable)
        {
            return DerivedLog<TEntry>.FromObservable(observable);
        }

        public static IEvent<TContext> AsEvent<TContext>(this IObservable<TContext> observable)
        {
            return DerivedEvent<TContext>.FromObservable(observable);
        }

        public static IEventLog<TContext> AsEventLog<TContext>(this IObservable<TContext> observable)
        {
            return DerivedEventLog<TContext>.FromObservable(observable);
        }

        public static ICounter<TIncrement> AsCounter<TIncrement>(this IObservable<TIncrement> observable)
        {
            return DerivedCounter<TIncrement>.FromObservable(observable);
        }

        public static IGauge<TValue> AsGauge<TValue>(this IObservable<TValue> observable)
        {
            return DerivedGauge<TValue>.FromObservable(observable);
        }

        public static IObservable<IList<TSource>> Buffer<TSource, TBufferClosing>(
            this IObservable<TSource> source,
            IOperation operation,
            Func<StartedOperation, IObservable<TBufferClosing>> bufferClosingSelector)
        {
            return source.Buffer(operation.Started, bufferClosingSelector);
        }
    }
}
