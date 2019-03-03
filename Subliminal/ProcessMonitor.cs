using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        private ProcessMonitor(IMetric<Process> processMetric)
        {
            ProcessMetric = processMetric;
        }

        public static ProcessMonitor ForProcess(System.Diagnostics.Process process, TimeSpan samplingInterval)
        {
            return new ProcessMonitor(Observable
                .Timer(DateTimeOffset.UtcNow, samplingInterval)
                .Select(_ => GetProcessSnapshot(process))
                .AsMetric());
        }

        public static ProcessMonitor ForCurrentProcess(TimeSpan samplingInterval)
        {
            return ForProcess(System.Diagnostics.Process.GetCurrentProcess(), samplingInterval);
        }

        private static Process GetProcessSnapshot(System.Diagnostics.Process process)
        {
            process.Refresh();

            return new Process(
                totalProcessorTime: process.TotalProcessorTime,
                workingSet: new SizeBytes(process.WorkingSet64),
                privateMemorySize: new SizeBytes(process.PrivateMemorySize64),
                virtualMemorySize: new SizeBytes(process.VirtualMemorySize64));
        }

        public IMetric<Process> ProcessMetric { get; }

        public IMetric<SizeBytes> PrivateMemorySizeMetric => ProcessMetric.Select(process => process.PrivateMemorySize).AsMetric();
        public IMetric<TimeSpan> TotalProcessorTimeMetric => ProcessMetric.Select(process => process.TotalProcessorTime).AsMetric();
        public IMetric<SizeBytes> VirtualMemorySizeMetric => ProcessMetric.Select(process => process.VirtualMemorySize).AsMetric();
        public IMetric<SizeBytes> WorkingSetMetric => ProcessMetric.Select(process => process.WorkingSet).AsMetric();

        public IMetric<ProcessorUsage> ProcessorUsageMetric
        {
            get
            {
                return ProcessMetric
                    .TimeInterval()
                    .Buffer(count: 2, skip: 1)
                    .Select(buffer => new ProcessorUsage(
                        processorTime: buffer[1].Value.TotalProcessorTime - buffer[0].Value.TotalProcessorTime,
                        interval: buffer[1].Interval,
                        processorCount: Environment.ProcessorCount))
                    .AsMetric();
            }
        }
    }
}
