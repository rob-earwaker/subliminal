using System;
using System.Linq;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedGauge<TValue> : IGauge<TValue>
    {
        private DerivedGauge(Guid gaugeId, IObservable<GaugeSample<TValue>> sampled)
        {
            GaugeId = gaugeId;
            Sampled = sampled;
        }

        public static DerivedGauge<TValue> FromLog(ILog<TValue> valueLog)
        {
            var gaugeId = Guid.NewGuid();

            var sampled = valueLog.EntryLogged
                .Select(entry => new GaugeSample<TValue>(gaugeId, entry.Value, entry.Timestamp, entry.Interval));

            return new DerivedGauge<TValue>(gaugeId, sampled);
        }

        public static DerivedGauge<TValue> FromObservable(IObservable<TValue> values)
        {
            return FromLog(values.AsLog());
        }

        public Guid GaugeId { get; }
        public IObservable<GaugeSample<TValue>> Sampled { get; }
    }
}
