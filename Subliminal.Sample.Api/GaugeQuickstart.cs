using System;

namespace Subliminal.Sample.Api
{
    internal class GaugeQuickstart : ISample
    {
        public void RunSample()
        {
            var gauge = new Gauge<double>();

            gauge.Subscribe(value =>
                Console.WriteLine($"Current gauge value is {value}"));

            gauge.LogValue(2.34);
            // "Current gauge value is 2.34"
            gauge.LogValue(59.41);
            // "Current gauge value is 59.41"
        }
    }
}
