namespace Subliminal.Events
{
    public class CounterIncremented
    {
        public CounterIncremented(int increment)
        {
            Increment = increment;
        }

        public int Increment { get; }
    }
}
