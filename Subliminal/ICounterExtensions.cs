using System.Reactive.Linq;

namespace Subliminal
{
    public static class ICounterExtensions
    {
        public static IGauge<Rate<TIncrement>> Rate<TIncrement>(this ICounter<TIncrement> counter)
        {
            return counter
                .TimeInterval()
                .Select(increment => new Rate<TIncrement>(increment.Value, increment.Interval))
                .AsGauge();
        }
    }
}
