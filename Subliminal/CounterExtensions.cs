namespace Subliminal
{
    /// <summary>
    /// Contains extensions for the <see cref="Counter{TIncrement}" /> type.
    /// </summary>
    public static class CounterExtensions
    {
        /// <summary>
        /// Captures an increment of one and emits it to all observers.
        /// </summary>
        public static void Increment(this Counter<int> counter)
        {
            counter.IncrementBy(1);
        }

        /// <summary>
        /// Captures an increment of one and emits it to all observers.
        /// </summary>
        public static void Increment(this Counter<long> counter)
        {
            counter.IncrementBy(1L);
        }
    }
}
