using System;
using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// Contains extensions for the <see cref="ICounter" /> interface.
    /// </summary>
    public static class ICounterExtensions
    {
        /// <summary>
        /// Measures the increment rate of a counter by recording the
        /// time interval between emitted increments.
        /// </summary>
        public static IObservable<Rate<double>> Rate(this ICounter counter)
        {
            return counter
                .TimeInterval()
                .Select(increment => new Rate<double>(
                    delta: increment.Value,
                    interval: increment.Interval));
        }
    }
}
