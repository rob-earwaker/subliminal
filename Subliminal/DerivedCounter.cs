using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedCounter : ICounter
    {
        private readonly ILog<long> _incrementLog;

        private DerivedCounter(ILog<long> incrementLog)
        {
            _incrementLog = incrementLog;
        }

        public static DerivedCounter FromObservable(IObservable<long> observable)
        {
            return new DerivedCounter(observable.Where(increment => increment > 0L).AsLog());
        }

        public Guid CounterId => _incrementLog.LogId;

        public IDisposable Subscribe(IObserver<long> observer)
        {
            return _incrementLog.Subscribe(observer);
        }
    }
}
