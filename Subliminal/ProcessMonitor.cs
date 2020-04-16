using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// A monitor that provides information about the state of a process.
    /// </summary>
    public sealed class ProcessMonitor
    {
        private ProcessMonitor(
            ILog<Process> process, ILog<string> standardOutput, ILog<string> standardError, IEvent<ProcessExited> exited)
        {
            Process = process;
            StandardOutput = standardOutput;
            StandardError = standardError;
            Exited = exited;

            PrivateMemorySize = process.Select(processSample => (double) processSample.PrivateMemorySize).AsGauge();
            VirtualMemorySize = process.Select(processSample => (double) processSample.VirtualMemorySize).AsGauge();
            WorkingSet = process.Select(processSample => (double) processSample.WorkingSet).AsGauge();
            //TotalProcessorTime = process.Select(processSample => (double) processSample.TotalProcessorTime).AsGauge();

            //ProcessorUsage = process
            //    .RateOfChange(processSample => processSample.TotalProcessorTime)
            //    .Select(rate => new ProcessorUsage(
            //        timeUsed: rate.Delta,
            //        interval: rate.Interval,
            //        processorCount: Environment.ProcessorCount))
            //    .AsGauge();
        }

        /// <summary>
        /// Create a monitor for a process. This will emit metrics containing information about the
        /// state of the process according to the sampling interval.
        /// </summary>
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
                    .AsLog(),
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

        /// <summary>
        /// Create a monitor for the current process. This will emit metrics containing information
        /// about the state of the process according to the sampling interval.
        /// </summary>
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

        /// <summary>
        /// A gauge representing the current state of the process.
        /// </summary>
        public ILog<Process> Process { get; }

        /// <summary>
        /// A log of all standard output text emitted by the process.
        /// </summary>
        public ILog<string> StandardOutput { get; }

        /// <summary>
        /// A log of all standard error text emitted by the process.
        /// </summary>
        public ILog<string> StandardError { get; }

        /// <summary>
        /// An event that is raised when the process is exited.
        /// </summary>
        public IEvent<ProcessExited> Exited { get; }

        /// <summary>
        /// A gauge representing the current private memory size of the process in bytes.
        /// </summary>
        public IGauge PrivateMemorySize { get; }

        /// <summary>
        /// A gauge representing the current virtual memory size of the process in bytes.
        /// </summary>
        public IGauge VirtualMemorySize { get; }


        /// <summary>
        /// A gauge representing the current working set size of the process in bytes.
        /// </summary>
        public IGauge WorkingSet { get; }

        /// <summary>
        /// A gauge representing the total processor time used by the process.
        /// </summary>
        public IGauge TotalProcessorTime { get; }

        /// <summary>
        /// A gauge representing the current processor usage of the process.
        /// </summary>
        public IGauge ProcessorUsage { get; }
    }
}
