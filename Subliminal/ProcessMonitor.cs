using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Subliminal
{
    public class ProcessMonitor
    {
        private ProcessMonitor(
            IGauge<Process> process, ILog<string> standardOutput, ILog<string> standardError, IEvent<ProcessExited> exited)
        {
            Process = process;
            StandardOutput = standardOutput;
            StandardError = standardError;
            Exited = exited;

            PrivateMemorySize = process.Select(processSample => processSample.PrivateMemorySize).AsGauge();
            VirtualMemorySize = process.Select(processSample => processSample.VirtualMemorySize).AsGauge();
            WorkingSet = process.Select(processSample => processSample.WorkingSet).AsGauge();
            TotalProcessorTime = process.Select(processSample => processSample.TotalProcessorTime).AsGauge();

            ProcessorUsage = process
                .Select(processSample => processSample.TotalProcessorTime)
                .AsGauge()
                .RateOfChange()
                .Select(rate => new ProcessorUsage(
                    timeUsed: rate.Delta,
                    interval: rate.Interval,
                    processorCount: Environment.ProcessorCount))
                .AsGauge();
        }

        public static ProcessMonitor ForProcess(System.Diagnostics.Process process, TimeSpan samplingInterval)
        {
            return new ProcessMonitor(
                process: Observable
                    .Interval(samplingInterval)
                    .Do(_ => process.Refresh())
                    .SkipWhile(_ => !HasStarted(process))
                    .TakeWhile(_ => !process.HasExited)
                    .Select(_ => new Process(
                        processId: process.Id,
                        totalProcessorTime: process.TotalProcessorTime,
                        workingSet: process.WorkingSet64,
                        privateMemorySize: process.PrivateMemorySize64,
                        virtualMemorySize: process.VirtualMemorySize64))
                    .AsGauge(),
                standardOutput: Observable
                    .FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                        handler => process.OutputDataReceived += handler,
                        handler => process.OutputDataReceived -= handler)
                    .Select(@event => @event.EventArgs.Data)
                    .AsLog(),
                standardError: Observable
                    .FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                        handler => process.ErrorDataReceived += handler,
                        handler => process.ErrorDataReceived -= handler)
                    .Select(@event => @event.EventArgs.Data)
                    .AsLog(),
                exited: Observable
                    .FromEventPattern(
                        handler => process.Exited += handler,
                        handler => process.Exited -= handler)
                    .Do(_ => process.Refresh())
                    .Select(_ => new ProcessExited(process.Id, process.ExitTime, process.ExitCode))
                    .AsEvent());
        }

        public static ProcessMonitor ForCurrentProcess(TimeSpan samplingInterval)
        {
            return ForProcess(System.Diagnostics.Process.GetCurrentProcess(), samplingInterval);
        }

        private static bool HasStarted(System.Diagnostics.Process process)
        {
            try
            {
                var processId = process.Id;
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public IGauge<Process> Process { get; }
        public ILog<string> StandardOutput { get; }
        public ILog<string> StandardError { get; }
        public IEvent<ProcessExited> Exited { get; }

        public IGauge<long> PrivateMemorySize { get; }
        public IGauge<long> VirtualMemorySize { get; }
        public IGauge<long> WorkingSet { get; }
        public IGauge<TimeSpan> TotalProcessorTime { get; }
        public IGauge<ProcessorUsage> ProcessorUsage { get; }
    }
}
