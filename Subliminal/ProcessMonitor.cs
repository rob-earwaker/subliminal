using Subliminal.Events;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        private readonly TimeSpan _samplingInterval;

        public ProcessMonitor(TimeSpan samplingInterval)
        {
            _samplingInterval = samplingInterval;

            ProcessGauge = new Gauge<Process>(
                Observable
                    .Interval(_samplingInterval)
                    .Select(_ => Process.GetCurrentProcess())
                    .AsGauge().Sampled
                    .Publish()
                    .AutoConnect());
        }

        public IGauge<Process> ProcessGauge { get; }

        public IGauge<long> PrivateMemorySize64Gauge => ProcessGauge.Select(process => process.PrivateMemorySize64);
        public IGauge<TimeSpan> TotalProcessorTimeGauge => ProcessGauge.Select(process => process.TotalProcessorTime);
        public IGauge<long> VirtualMemorySize64Gauge => ProcessGauge.Select(process => process.VirtualMemorySize64);
        public IGauge<long> WorkingSet64Gauge => ProcessGauge.Select(process => process.WorkingSet64);

        public IGauge<double> CpuUsageGauge
        {
            get
            {
                return TotalProcessorTimeGauge.Sampled
                    .Buffer(count: 2, skip: 1)
                    .Select(buffer =>
                        new GaugeSampled<double>(
                            value: CalculateCpuUsage(
                                startTotalUsage: buffer[0].Value,
                                endTotalUsage: buffer[1].Value,
                                interval: buffer[1].Interval),
                            timestamp: buffer[1].Timestamp,
                            interval: buffer[1].Interval))
                    .AsGauge();
            }
        }

        private double CalculateCpuUsage(TimeSpan startTotalUsage, TimeSpan endTotalUsage, TimeSpan interval)
        {
            var intervaUsage = endTotalUsage - startTotalUsage;
            return intervaUsage.TotalMilliseconds / interval.TotalMilliseconds / Environment.ProcessorCount;
        }
    }
}
