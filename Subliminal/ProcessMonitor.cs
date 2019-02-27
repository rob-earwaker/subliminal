using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        public ProcessMonitor(TimeSpan samplingInterval)
        {
            ProcessSource = Observable
                .Interval(samplingInterval)
                .Select(_ => Process.GetCurrentProcess())
                .AsSampleSource();
        }

        public ISampleSource<Process> ProcessSource { get; }

        public ISampleSource<long> PrivateMemorySize64Source => ProcessSource.Select(process => process.PrivateMemorySize64);
        public ISampleSource<TimeSpan> TotalProcessorTimeSource => ProcessSource.Select(process => process.TotalProcessorTime);
        public ISampleSource<long> VirtualMemorySize64Source => ProcessSource.Select(process => process.VirtualMemorySize64);
        public ISampleSource<long> WorkingSet64Source => ProcessSource.Select(process => process.WorkingSet64);

        public ISampleSource<double> CpuUsageSource
        {
            get
            {
                return TotalProcessorTimeSource
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
