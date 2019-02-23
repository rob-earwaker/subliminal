namespace Subliminal.Events
{
    public class GaugeSampled<TValue>
    {
        public GaugeSampled(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }
    }
}
