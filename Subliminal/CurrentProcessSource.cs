using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public class CurrentProcessSource : ISource<Process>
    {
        private readonly ISource<Process> _source;

        public CurrentProcessSource(TimeSpan samplingInterval)
        {
            _source = Observable
                .Interval(samplingInterval)
                .Select(_ => Process.FromCurrentProcess())
                .AsSource();
        }

        public IObservable<Observation<Process>> Observations => _source.Observations;

        public ISource<long> PrivateMemorySize => _source.Select(process => process.PrivateMemorySize);
        public ISource<TimeSpan> TotalProcessorTime => _source.Select(process => process.TotalProcessorTime);
        public ISource<long> VirtualMemorySize => _source.Select(process => process.VirtualMemorySize);
        public ISource<long> WorkingSet => _source.Select(process => process.WorkingSet);

        public ISource<ProcessorUsage> ProcessorUsage
        {
            get
            {
                return _source
                    .Buffer(count: 2, skip: 1)
                    .Select(buffer => new ProcessorUsage(
                        fraction: CalculateProcessorUsageFraction(
                            startTotalUsage: buffer[0].ObservedValue.TotalProcessorTime,
                            endTotalUsage: buffer[1].ObservedValue.TotalProcessorTime,
                            interval: buffer[1].Interval),
                        processorCount: Environment.ProcessorCount));
            }
        }

        public ISource<IList<Observation<Process>>> Buffer(int count, int skip)
        {
            return _source.Buffer(count, skip);
        }

        public ISource<TNewValue> Select<TNewValue>(Func<Process, TNewValue> selector)
        {
            return _source.Select(selector);
        }

        public IDisposable Subscribe(Action<Observation<Process>> onNext)
        {
            return _source.Subscribe(onNext);
        }

        private double CalculateProcessorUsageFraction(TimeSpan startTotalUsage, TimeSpan endTotalUsage, TimeSpan interval)
        {
            var intervalUsage = endTotalUsage - startTotalUsage;
            return intervalUsage.TotalMilliseconds / interval.TotalMilliseconds;
        }
    }
}
