namespace Subliminal
{
    public static class CounterExtensions
    {
        public static void Increment(this Counter<int> counter)
        {
            counter.IncrementBy(1);
        }

        public static void Increment(this Counter<long> counter)
        {
            counter.IncrementBy(1L);
        }
    }
}
