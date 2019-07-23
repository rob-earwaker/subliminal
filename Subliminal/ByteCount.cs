namespace Subliminal
{
    public class ByteCount
    {
        private const double DecimalMultiple = 1000.0;
        private const double BinaryMultiple = 1024.0;

        public ByteCount(long byteCount)
        {
            Bytes = byteCount;

            Kilobytes = byteCount / DecimalMultiple;
            Megabytes = Kilobytes / DecimalMultiple;
            Gigabytes = Megabytes / DecimalMultiple;

            Kibibytes = byteCount / BinaryMultiple;
            Mebibytes = Kibibytes / BinaryMultiple;
            Gibibytes = Mebibytes / BinaryMultiple;
        }

        public long Bytes { get; }

        public double Kilobytes { get; }
        public double Megabytes { get; }
        public double Gigabytes { get; }

        public double Kibibytes { get; }
        public double Mebibytes { get; }
        public double Gibibytes { get; }
    }
}
