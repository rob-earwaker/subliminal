using System;

namespace Subliminal
{
    public sealed class DerivedCounter<TIncrement> : ICounter<TIncrement>
    {
        private readonly ILog<TIncrement> _incrementLog;

        private DerivedCounter(ILog<TIncrement> incrementLog)
        {
            _incrementLog = incrementLog;
        }

        public static DerivedCounter<TIncrement> FromObservable(IObservable<TIncrement> observable)
        {
            return new DerivedCounter<TIncrement>(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<TIncrement> observer)
        {
            return _incrementLog.Subscribe(observer);
        }
    }
}
