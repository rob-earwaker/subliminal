using System;

namespace Subliminal.Events
{
    public class GaugeSampled<TGauge> : EventArgs
    {
        public GaugeSampled(TGauge value)
        {
            Value = value;
        }

        public TGauge Value { get; }
    }
}