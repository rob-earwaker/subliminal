using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        private ProcessMonitor(IMetric<Process> process, IEvent<ProcessExited> exited)
        {
            Process = process;
            Exited = exited;
        }

        public static ProcessMonitor ForProcess(System.Diagnostics.Process process, TimeSpan samplingInterval)
        {
            process.EnableRaisingEvents = true;

            return new ProcessMonitor(
                process: Observable
                    .Timer(DateTimeOffset.UtcNow, samplingInterval)
                    .TakeWhile(_ => !process.HasExited)
                    .Select(_ => GetProcessSnapshot(process))
                    .AsMetric(),
                exited: Observable
                    .FromEventPattern(
                        eventHandler => process.Exited += eventHandler,
                        eventHandler => process.Exited -= eventHandler)
                    .Take(1)
                    .Select(_ => new ProcessExited(process.Id, process.ExitTime, process.ExitCode))
                    .AsEvent());
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
                workingSet: new ByteCount(process.WorkingSet64),
                privateMemorySize: new ByteCount(process.PrivateMemorySize64),
                virtualMemorySize: new ByteCount(process.VirtualMemorySize64));
        }

        public IMetric<Process> Process { get; }
        public IEvent<ProcessExited> Exited { get; }

        public IMetric<ByteCount> PrivateMemorySize => Process.Select(process => process.PrivateMemorySize).AsMetric();
        public IMetric<TimeSpan> TotalProcessorTime => Process.Select(process => process.TotalProcessorTime).AsMetric();
        public IMetric<ByteCount> VirtualMemorySize => Process.Select(process => process.VirtualMemorySize).AsMetric();
        public IMetric<ByteCount> WorkingSet => Process.Select(process => process.WorkingSet).AsMetric();

        public IMetric<ProcessorUsage> ProcessorUsage
        {
            get
            {
                return Process
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
