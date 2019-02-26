using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        public ProcessMonitor(TimeSpan samplingInterval)
        {
            ProcessGauge = Observable
                .Interval(samplingInterval)
                .Select(_ => Process.GetCurrentProcess())
                .AsGauge();
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
                return TotalProcessorTimeGauge
                    .Buffer(count: 2, skip: 1)
                    .Select(buffer => CalculateCpuUsage(
                        startTotalUsage: buffer[0].Value,
                        endTotalUsage: buffer[1].Value,
                        interval: buffer[1].Interval));
            }
        }

        private double CalculateCpuUsage(TimeSpan startTotalUsage, TimeSpan endTotalUsage, TimeSpan interval)
        {
            var intervaUsage = endTotalUsage - startTotalUsage;
            return intervaUsage.TotalMilliseconds / interval.TotalMilliseconds / Environment.ProcessorCount;
        }
    }
}
