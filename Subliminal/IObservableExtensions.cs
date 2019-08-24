using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IObservableExtensions
    {
        public static ILog<TEntry> AsLog<TEntry>(this IObservable<TEntry> observable)
        {
            return DerivedLog<TEntry>.FromObservable(observable);
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

        public static ICounter<TIncrement> AsCounter<TIncrement>(this IObservable<TIncrement> observable)
        {
            return DerivedCounter<TIncrement>.FromObservable(observable);
        }

        public static IGauge<TValue> AsGauge<TValue>(this IObservable<TValue> observable)
        {
            return DerivedGauge<TValue>.FromObservable(observable);
        }

        public static IObservable<int> Delta(this IObservable<int> observable)
        {
            return observable.Delta(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<long> Delta(this IObservable<long> observable)
        {
            return observable.Delta(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<TimeSpan> Delta(this IObservable<TimeSpan> observable)
        {
            return observable.Delta(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Delta<TValue>> Delta<TValue>(this IObservable<TValue> observable)
        {
            return observable.Delta(delta => delta);
        }

        public static IObservable<TDelta> Delta<TValue, TDelta>(
            this IObservable<TValue> observable, Func<Delta<TValue>, TDelta> deltaSelector)
        {
            return observable
                .Buffer(count: 2, skip: 1)
                // If the observable completes, the last item will be emitted in a buffer containing just
                // that item. This will have already been emitted as the second value in the previous buffer,
                // so ignoring this smaller final buffer does not hide the final value from subscribers.
                .Where(buffer => buffer.Count == 2)
                .Select(buffer => deltaSelector(new Delta<TValue>(previousValue: buffer[0], currentValue: buffer[1])));
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

        public static IObservable<Rate<long>> RateOfChange(this IObservable<long> observable)
        {
            return observable.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Rate<TimeSpan>> RateOfChange(this IObservable<TimeSpan> observable)
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
            return observable.Select(_ => 1L);
        }
    }
}
