using System;

namespace Subliminal
{
    /// <summary>
    /// Represents the rate at which a value has changed between
    /// consecutive values in a sequence.
    /// </summary>
    public sealed class Rate
    {
        /// <summary>
        /// Creates a representation of the rate at which a value has changed
        /// between consecutive values in a sequence.
        /// </summary>
        public Rate(double delta, TimeSpan interval)
        {
            Delta = delta;
            Interval = interval;
        }

        /// <summary>
        /// The amount the value has changed since the previous value in the sequence.
        /// </summary>
        public double Delta { get; }

        /// <summary>
        /// The elapsed time since the previous value in the sequence.
        /// </summary>
        public TimeSpan Interval { get; }
    }
}