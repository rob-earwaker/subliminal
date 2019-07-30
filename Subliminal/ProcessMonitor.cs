using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        private ProcessMonitor(IGauge<Process> process, IEvent<ExitedProcess> exited)
        {
            Process = process;
            Exited = exited;

            PrivateMemorySize = process.Select(processSample => processSample.PrivateMemorySize).AsGauge();
            TotalProcessorTime = process.Select(processSample => processSample.TotalProcessorTime).AsGauge();
            VirtualMemorySize = process.Select(processSample => processSample.VirtualMemorySize).AsGauge();
            WorkingSet = process.Select(processSample => processSample.WorkingSet).AsGauge();

            CpuUsage = process
                .TimeInterval()
                .Buffer(count: 2, skip: 1)
                .Select(buffer => new ProcessorUsage(
                    timeUsed: buffer[1].Value.TotalProcessorTime - buffer[0].Value.TotalProcessorTime,
                    interval: buffer[1].Interval,
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
                    .Select(_ => new ExitedProcess(process.Id, process.ExitTime, process.ExitCode))
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
                workingSet: ByteCount.FromBytes(process.WorkingSet64),
                privateMemorySize: ByteCount.FromBytes(process.PrivateMemorySize64),
                virtualMemorySize: ByteCount.FromBytes(process.VirtualMemorySize64));
        }

        public IGauge<Process> Process { get; }
        public IEvent<ExitedProcess> Exited { get; }
        public IGauge<ByteCount> PrivateMemorySize { get; }
        public IGauge<TimeSpan> TotalProcessorTime { get; }
        public IGauge<ByteCount> VirtualMemorySize { get; }
        public IGauge<ByteCount> WorkingSet { get; }
        public IGauge<ProcessorUsage> CpuUsage { get; }
    }
}
