using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedCounter : ICounter
    {
        private readonly ILog<int> _log;

        private DerivedCounter(ILog<int> log)
        {
            _log = log;
        }

        public static DerivedCounter FromObservable(IObservable<int> observable)
        {
            return new DerivedCounter(observable.Where(increment => increment > 0).AsLog());
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
