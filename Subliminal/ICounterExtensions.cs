using System;
using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// Contains extensions for the <see cref="ICounter{TIncrement}" /> interface.
    /// </summary>
    public static class ICounterExtensions
    {
        /// <summary>
        /// Measures the increment rate of a counter by recording the
        /// time interval between emitted increments.
        /// </summary>
        public static IObservable<Rate<TIncrement>> Rate<TIncrement>(this ICounter<TIncrement> counter)
        {
            return counter.Rate(increment => increment);
        }

        /// <summary>
        /// Measures the increment rate of a counter by recording the
        /// time interval between emitted increments.
        /// </summary>
        public static IObservable<Rate<TDelta>> Rate<TIncrement, TDelta>(
            this ICounter<TIncrement> counter, Func<TIncrement, TDelta> incrementSelector)
        {
            return counter
                .TimeInterval()
                .Select(increment => new Rate<TDelta>(
                    delta: incrementSelector(increment.Value),
                    interval: increment.Interval));
        }
    }
}
