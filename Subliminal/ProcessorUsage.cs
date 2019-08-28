using System;

namespace Subliminal
{
    /// <summary>
    /// A metric containing processor usage information over a period of time.
    /// </summary>
    public sealed class ProcessorUsage
    {
        /// <summary>
        /// Creates a metric containing processor usage information over a period of time.
        /// </summary>
        public ProcessorUsage(TimeSpan timeUsed, TimeSpan interval, int processorCount)
        {
            TimeUsed = timeUsed;
            Interval = interval;
            ProcessorCount = processorCount;
        }

        /// <summary>
        /// The processor time used over the sample period.
        /// </summary>
        public TimeSpan TimeUsed { get; }

        /// <summary>
        /// The sample period duration.
        /// </summary>
        public TimeSpan Interval { get; }

        /// <summary>
        /// The number of processors available.
        /// </summary>
        public int ProcessorCount { get; }

        /// <summary>
        /// The fraction of a single processor's available time that was used during the sample period.
        /// </summary>
        public double Fraction => TimeUsed.TotalMilliseconds / TimeAvailable.TotalMilliseconds;

        /// <summary>
        /// The percentage of a single processor's available time that was used during the sample period.
        /// </summary>
        public double Percentage => Fraction * 100.0;

        /// <summary>
        /// The processing time available from a single processor during the sample period.
        /// </summary>
        public TimeSpan TimeAvailable => Interval;

        /// <summary>
        /// The fraction of all processors' available time that was used during the sample period.
        /// </summary>
        public double TotalFraction => Fraction / ProcessorCount;

        /// <summary>
        /// The percentage of all processors' available time that was used during the sample period.
        /// </summary>
        public double TotalPercentage => Percentage / ProcessorCount;

        /// <summary>
        /// The processing time available across all processors during the sample period.
        /// </summary>
        public TimeSpan TotalTimeAvailable => TimeSpan.FromTicks(TimeAvailable.Ticks * ProcessorCount);
    }
}
