namespace Subliminal
{
    public class SizeBytes
    {
        public SizeBytes(long bytes)
        {
            Bytes = bytes;
        }

        public long Bytes { get; }
        public double MegaBytes => Bytes / 1000;
        public double GigaBytes => MegaBytes / 1000;
    }
}
