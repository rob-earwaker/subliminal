namespace Subliminal
{
    public static class ILogExtensions
    {
        public static ICounter<TIncrement> AsCounter<TIncrement>(this ILog<TIncrement> log)
        {
            return DerivedCounter<TIncrement>.FromLog(log);
        }

        public static IEventLog<TEvent> AsEventLog<TEvent>(this ILog<TEvent> log)
        {
            return DerivedEventLog<TEvent>.FromLog(log);
        }

        public static IGauge<TValue> AsGauge<TValue>(this ILog<TValue> log)
        {
            return DerivedGauge<TValue>.FromLog(log);
        }
    }
}
