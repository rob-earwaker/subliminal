using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        private ProcessMonitor(IGauge<Process> process, ITrigger<ProcessExited> exited)
        {
            Process = process;
            Exited = exited;

            PrivateMemorySize = process.Sampled.Select(processSample => processSample.Value.PrivateMemorySize).AsGauge();
            TotalProcessorTime = process.Sampled.Select(processSample => processSample.Value.TotalProcessorTime).AsGauge();
            VirtualMemorySize = process.Sampled.Select(processSample => processSample.Value.VirtualMemorySize).AsGauge();
            WorkingSet = process.Sampled.Select(processSample => processSample.Value.WorkingSet).AsGauge();

            CpuUsage = process.Sampled
                .Buffer(count: 2, skip: 1)
                .Select(buffer => new ProcessorUsage(
                    timeUsed: buffer[1].Value.TotalProcessorTime - buffer[0].Value.TotalProcessorTime,
                    maxTimeAvailablePerProcessor: buffer[1].Interval,
                    processorCount: Environment.ProcessorCount))
                .AsGauge();
        }

        public static ProcessMonitor ForProcess(System.Diagnostics.Process process, TimeSpan samplingInterval)
        {
            process.EnableRaisingEvents = true;

            return new ProcessMonitor(
                process: Observable
                    .Timer(DateTimeOffset.UtcNow, samplingInterval)
                    .TakeWhile(_ => !process.HasExited)
                    .Select(_ => GetProcessSnapshot(process))
                    .AsGauge(),
                exited: Observable
                    .FromEventPattern(
                        eventHandler => process.Exited += eventHandler,
                        eventHandler => process.Exited -= eventHandler)
                    .Take(1)
                    .Select(_ => new ProcessExited(process.Id, process.ExitTime, process.ExitCode))
                    .AsTrigger());
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

        public IGauge<Process> Process { get; }
        public ITrigger<ProcessExited> Exited { get; }
        public IGauge<ByteCount> PrivateMemorySize { get; }
        public IGauge<TimeSpan> TotalProcessorTime { get; }
        public IGauge<ByteCount> VirtualMemorySize { get; }
        public IGauge<ByteCount> WorkingSet { get; }
        public IGauge<ProcessorUsage> CpuUsage { get; }
    }
}
