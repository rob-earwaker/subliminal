namespace Subliminal
{
    /// <summary>
    /// Represents the delta between an observed value and the
    /// previous value in the sequence.
    /// </summary>
    public sealed class Delta<TValue>
    {
        /// <summary>
        /// Creates a representation of the delta between an observed value
        /// and the previous value in the sequence.
        /// </summary>
        public Delta(TValue previousValue, TValue currentValue)
        {
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }

        /// <summary>
        /// The previous value in the observable sequence.
        /// </summary>
        public TValue PreviousValue { get; }

        /// <summary>
        /// The current value in the observable sequence.
        /// </summary>
        public TValue CurrentValue { get; }
    }
}
