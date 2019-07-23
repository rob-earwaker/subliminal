using System;

namespace Subliminal
{
    public class BitRate
    {
        public BitRate(ByteCount count, TimeSpan interval)
        {
            Count = count;
            Interval = interval;
        }

        public ByteCount Count { get; }
        public TimeSpan Interval { get; }

        public double BitsPerSecond => Count.Bits / Interval.TotalSeconds;
        public double KilobitsPerSecond => Count.Kilobits / Interval.TotalSeconds;
        public double MegabitsPerSecond => Count.Megabits / Interval.TotalSeconds;

        public double BytesPerSecond => Count.Bytes / Interval.TotalSeconds;
        public double KilobytesPerSecond => Count.Kilobytes / Interval.TotalSeconds;
        public double MegabytesPerSecond => Count.Megabytes / Interval.TotalSeconds;
    }
}
