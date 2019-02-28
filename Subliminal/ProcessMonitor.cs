using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        public ProcessMonitor(TimeSpan samplingInterval)
        {
            Process = Observable
                .Interval(samplingInterval)
                .Select(_ => Subliminal.Process.FromCurrentProcess())
                .AsSource();
        }

        public ISource<Process> Process { get; }

        public ISource<long> PrivateMemorySize => Process.Select(process => process.PrivateMemorySize);
        public ISource<TimeSpan> TotalProcessorTime => Process.Select(process => process.TotalProcessorTime);
        public ISource<long> VirtualMemorySize => Process.Select(process => process.VirtualMemorySize);
        public ISource<long> WorkingSet => Process.Select(process => process.WorkingSet);

        public ISource<ProcessorUsage> ProcessorUsage
        {
            get
            {
                return Process
                    .Buffer(count: 2, skip: 1)
                    .Select(buffer => new ProcessorUsage(
                        fraction: CalculateProcessorUsageFraction(
                            startTotalUsage: buffer[0].Value.TotalProcessorTime,
                            endTotalUsage: buffer[1].Value.TotalProcessorTime,
                            interval: buffer[1].Interval),
                        processorCount: Environment.ProcessorCount));
            }
        }

        private double CalculateProcessorUsageFraction(TimeSpan startTotalUsage, TimeSpan endTotalUsage, TimeSpan interval)
        {
            var intervalUsage = endTotalUsage - startTotalUsage;
            return intervalUsage.TotalMilliseconds / interval.TotalMilliseconds;
        }
    }
}
