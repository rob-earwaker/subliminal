namespace Subliminal
{
    public static class ILogExtensions
    {
        public static ICounter<TIncrement> AsCounter<TIncrement>(this ILog<TIncrement> log)
        {
            return DerivedCounter<TIncrement>.FromLog(log);
        }

        public static IEventLog<TContext> AsEventLog<TContext>(this ILog<TContext> log)
        {
            return DerivedEventLog<TContext>.FromLog(log);
        }

        public static IGauge<TValue> AsGauge<TValue>(this ILog<TValue> log)
        {
            return DerivedGauge<TValue>.FromLog(log);
        }
    }
}
