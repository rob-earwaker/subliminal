namespace Subliminal

open System
open System.Diagnostics
open System.Reactive.Linq

/// A metric containing information about a running process.
type Process
    /// <summary>A metric containing information about a running process.</summary>
    /// <param name="processId">The PID of the process.</param>
    /// <param name="totalProcessorTime">The total processor time used by the process.</param>
    /// <param name="workingSet">The current working set usage of the process in bytes.</param>
    /// <param name="privateMemorySize">The current private memory size of the process in bytes.</param>
    /// <param name="virtualMemorySize">The current virtual memory size of the process in bytes.</param>
    internal (processId: int, totalProcessorTime: TimeSpan,
              workingSet: int64, privateMemorySize: int64, virtualMemorySize: int64) =

    /// The PID of the process.
    member val ProcessId = processId
    /// The total processor time used by the process.
    member val TotalProcessorTime = totalProcessorTime
    /// The current working set usage of the process in bytes.
    member val WorkingSet = workingSet
    /// The current private memory size of the process in bytes.
    member val PrivateMemorySize = privateMemorySize
    /// The current virtual memory size of the process in bytes.
    member val VirtualMemorySize = virtualMemorySize

/// A metric containing processor usage information over a period of time.
type ProcessExited
    /// A metric containing processor usage information over a period of time.
    internal (processId: int, exitTime: DateTime, exitCode: int) =
    member val ProcessId = processId
    member val ExitTime = exitTime
    member val ExitCode = exitCode

/// A metric containing processor usage information over a period of time.
type ProcessorUsage
    /// <summary>A metric containing processor usage information over a period of time.</summary>
    /// <param name="timeUsed">The processor time used over the sample period.</param>
    /// <param name="interval">The sample period duration.</param>
    /// <param name="processorCount">The number of processors available.</param>
    internal (timeUsed: TimeSpan, interval: TimeSpan, processorCount: int) =

    /// The processor time used over the sample period.
    member val TimeUsed = timeUsed
    /// The sample period duration.
    member val Interval = interval
    /// The number of processors available.
    member val ProcessorCount = processorCount

    /// The processing time available from a single processor during the sample period.
    member this.TimeAvailable = this.Interval
    /// The fraction of a single processor's available time that was used during the sample period.
    member this.Fraction = this.TimeUsed.TotalMilliseconds / this.TimeAvailable.TotalMilliseconds
    /// The percentage of a single processor's available time that was used during the sample period.
    member this.Percentage = this.Fraction * 100.0

    /// The processing time available across all processors during the sample period.
    member this.TotalTimeAvailable = TimeSpan.FromTicks(this.TimeAvailable.Ticks * int64 this.ProcessorCount)
    /// The fraction of all processors' available time that was used during the sample period.
    member this.TotalFraction = this.Fraction / float this.ProcessorCount
    /// The percentage of all processors' available time that was used during the sample period.
    member this.TotalPercentage = this.Percentage / float this.ProcessorCount

/// A monitor that provides information about the state of a process.
type ProcessMonitor private
    (processInfo: ILog<Process>, exited: IEvent<ProcessExited>,
     standardOutput: ILog<string>, standardError: ILog<string>) =

    static member ForProcess(process': Diagnostics.Process, samplingInterval) =
        let processInfo =
            Observable.Interval(samplingInterval)
            |> fun obs -> obs.Do(fun _ -> process'.Refresh())
            |> fun obs -> obs.SkipWhile(fun _ -> not (ProcessMonitor.HasStarted(process')))
            |> fun obs -> obs.TakeWhile(fun _ -> not process'.HasExited)
            |> Observable.map (fun _ ->
                Process(
                    process'.Id,
                    process'.TotalProcessorTime,
                    process'.WorkingSet64,
                    process'.PrivateMemorySize64,
                    process'.VirtualMemorySize64))
            |> Log.ofObservable
        let exited =
            Observable.FromEventPattern(process'.Exited.AddHandler, process'.Exited.RemoveHandler)
            |> fun obs -> obs.Do(fun _ -> process'.Refresh())
            |> Observable.map (fun _ -> ProcessExited(process'.Id, process'.ExitTime, process'.ExitCode))
            |> Log.ofObservable
            |> Event.ofLog'
        let standardOutput =
            Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                process'.OutputDataReceived.AddHandler,
                process'.OutputDataReceived.RemoveHandler)
            |> Observable.map (fun event -> event.EventArgs.Data)
            |> Log.ofObservable
        let standardError =
            Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                process'.ErrorDataReceived.AddHandler,
                process'.ErrorDataReceived.RemoveHandler)
            |> Observable.map (fun event -> event.EventArgs.Data)
            |> Log.ofObservable
        ProcessMonitor(processInfo, exited, standardOutput, standardError)

    static member ForCurrentProcess(samplingInterval) =
        ProcessMonitor.ForProcess(Diagnostics.Process.GetCurrentProcess(), samplingInterval)

    static member private HasStarted(process': Diagnostics.Process) =
        try
            do process'.Id |> ignore
            true
        with
        | :? InvalidOperationException -> false
