using System;

namespace Lognostics.Events
{
    public class GaugeSampled<TGauge> : EventArgs
    {
        public GaugeSampled(Guid gaugeId, TGauge value)
        {
            GaugeId = gaugeId;
            Value = value;
        }

        public Guid GaugeId { get; }
        public TGauge Value { get; }
    }
}