using System;

namespace Lognostics.Events
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