namespace Subliminal
{
    public class ByteCount
    {
        public static readonly ByteCount Zero = new ByteCount(0L);

        private const double DecimalMultiplier = 1000.0;
        private const double KiloMultiplier = DecimalMultiplier;
        private const double MegaMultiplier = DecimalMultiplier * KiloMultiplier;
        private const double GigaMultiplier = DecimalMultiplier * MegaMultiplier;

        private const double BinaryMultiplier = 1024.0;
        private const double KibiMultiplier = BinaryMultiplier;
        private const double MebiMultiplier = BinaryMultiplier * KiloMultiplier;
        private const double GibiMultiplier = BinaryMultiplier * MegaMultiplier;

        private const int BitsPerByte = 8;

        private ByteCount(long byteCount)
        {
            Bytes = byteCount;
        }

        public static ByteCount FromBytes(long byteCount)
        {
            return new ByteCount(byteCount);
        }

        public long Bytes { get; }

        public double Kilobytes => Bytes / KiloMultiplier;
        public double Megabytes => Bytes / MegaMultiplier;
        public double Gigabytes => Bytes / GigaMultiplier;

        public double Kibibytes => Bytes / KibiMultiplier;
        public double Mebibytes => Bytes / MebiMultiplier;
        public double Gibibytes => Bytes / GibiMultiplier;

        public long Bits => Bytes * BitsPerByte;

        public double Kilobits => Kilobytes * BitsPerByte;
        public double Megabits => Megabytes * BitsPerByte;
        public double Gigabits => Gigabytes * BitsPerByte;

        public double Kibibits => Kibibytes * BitsPerByte;
        public double Mebibits => Mebibytes * BitsPerByte;
        public double Gibibits => Gibibytes * BitsPerByte;

        public static ByteCount operator +(ByteCount byteCount1, ByteCount byteCount2)
        {
            return ByteCount.FromBytes(byteCount1.Bytes + byteCount2.Bytes);
        }
    }
}
