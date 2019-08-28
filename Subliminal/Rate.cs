using System;

namespace Subliminal
{
    /// <summary>
    /// Represents the rate at which a value has changed between an observed
    /// value and the previous value in the sequence.
    /// </summary>
    public sealed class Rate<TDelta>
    {
        /// <summary>
        /// Creates a representation of the rate at which a value has changed
        /// between an observed value and the previous value in the sequence.
        /// </summary>
        public Rate(TDelta delta, TimeSpan interval)
        {
            Delta = delta;
            Interval = interval;
        }

        /// <summary>
        /// The amount the value has changed since the previous value in the sequence.
        /// </summary>
        public TDelta Delta { get; }

        /// <summary>
        /// The elapsed time since the previous value in the sequence.
        /// </summary>
        public TimeSpan Interval { get; }
    }
}