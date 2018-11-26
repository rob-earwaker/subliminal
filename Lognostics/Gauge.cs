using Lognostics.Events;
using System;

namespace Lognostics
{
    public class Gauge<TGauge>
    {
        public event EventHandler<GaugeSampled<TGauge>> Sampled;

        public void LogValue(TGauge value)
        {
            Sampled?.Invoke(this, new GaugeSampled<TGauge>(value));
        }
    }
}
