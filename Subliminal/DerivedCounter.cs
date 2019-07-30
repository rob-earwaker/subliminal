using System;

namespace Subliminal
{
    public class DerivedCounter<TIncrement> : ICounter<TIncrement>
    {
        private readonly IObservable<TIncrement> _incremented;

        private DerivedCounter(IObservable<TIncrement> incremented)
        {
            _incremented = incremented;
        }

        public static DerivedCounter<TIncrement> FromLog(ILog<TIncrement> log)
        {
            return new DerivedCounter<TIncrement>(log);
        }

        public static DerivedCounter<TIncrement> FromObservable(IObservable<TIncrement> observable)
        {
            return FromLog(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<TIncrement> observer)
        {
            return _incremented.Subscribe(observer);
        }
    }
}
