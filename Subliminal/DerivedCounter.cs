using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedCounter : ICounter
    {
        private DerivedCounter(Guid counterId, IObservable<CounterIncrement> incremented)
        {
            CounterId = counterId;
            Incremented = incremented;
        }

        public static DerivedCounter FromLog(ILog<long> incrementLog)
        {
            var counterId = Guid.NewGuid();

            var incremented = incrementLog.EntryLogged
                .Where(entry => entry.Value > 0L)
                .Select(entry => new CounterIncrement(counterId, entry.Value, entry.Timestamp, entry.Interval));

            return new DerivedCounter(counterId, incremented);
        }

        public static DerivedCounter FromIncrements(IObservable<long> increments)
        {
            return FromLog(increments.AsLog());
        }

        public Guid CounterId { get; }
        public IObservable<CounterIncrement> Incremented { get; }
    }
}
