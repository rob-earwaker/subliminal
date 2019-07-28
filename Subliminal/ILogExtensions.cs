using System.Reactive;

namespace Subliminal
{
    public static class ILogExtensions
    {
        public static ICounter AsCounter(this ILog<long> incrementLog)
        {
            return DerivedCounter.FromLog(incrementLog);
        }

        public static IEventLog AsEventLog(this ILog<Unit> occurrenceLog)
        {
            return DerivedEventLog.FromLog(occurrenceLog);
        }

        public static IEventLog<TContext> AsEventLog<TContext>(this ILog<TContext> contextLog)
        {
            return DerivedEventLog<TContext>.FromLog(contextLog);
        }

        public static IGauge<TValue> AsGauge<TValue>(this ILog<TValue> valueLog)
        {
            return DerivedGauge<TValue>.FromLog(valueLog);
        }
    }
}
