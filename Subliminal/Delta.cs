namespace Subliminal
{
    public sealed class Delta<TValue>
    {
        public Delta(TValue previousValue, TValue currentValue)
        {
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }

        public TValue PreviousValue { get; }
        public TValue CurrentValue { get; }
    }
}
