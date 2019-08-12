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
        public static IObservable<Rate<TValue>> Rate<TValue>(this IObservable<TValue> observable)
        {
            return observable
                .TimeInterval()
                .Select(increment => new Rate<TValue>(increment.Value, increment.Interval));
        }

        public static IObservable<Rate<int>> RateOfChange(this IObservable<int> observable)
        {
            return observable.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Rate<TimeSpan>> RateOfChange(this IObservable<TimeSpan> observable)
        {
            return observable.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Rate<ByteCount>> RateOfChange(this IObservable<ByteCount> observable)
        {
            return observable.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Rate<Delta<TValue>>> RateOfChange<TValue>(this IObservable<TValue> observable)
        {
            return observable.RateOfChange(delta => delta);
        }

        public static IObservable<Rate<TDelta>> RateOfChange<TValue, TDelta>(
            this IObservable<TValue> observable, Func<Delta<TValue>, TDelta> deltaSelector)
        {
            return observable
                .TimeInterval()
                .Buffer(count: 2, skip: 1)
                .Select(buffer => new Rate<TDelta>(
                    delta: deltaSelector(new Delta<TValue>(previousValue: buffer[0].Value, currentValue: buffer[1].Value)),
                    interval: buffer[1].Interval));
        }
        public static IObservable<long> SampleCount<TValue>(this IObservable<TValue> observable)
        {
            return observable.Select(context => 1L);
        }
    }
}
