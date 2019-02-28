namespace Subliminal
{
    public class ProcessorUsage
    {
        public ProcessorUsage(double fraction, int processorCount)
        {
            Fraction = fraction;
            ProcessorCount = processorCount;
        }

        public double Fraction { get; }
        public int ProcessorCount { get; }

        public double Percentage => Fraction * 100;
    }
}
