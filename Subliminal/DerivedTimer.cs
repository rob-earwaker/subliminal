using System;

namespace Subliminal
{
    public class DerivedTimer : ITimer
    {
        private readonly ILog<TimeSpan> _durationLog;

        private DerivedTimer(ILog<TimeSpan> durationLog)
        {
            _durationLog = durationLog;
        }

        public static DerivedTimer FromObservable(IObservable<TimeSpan> observable)
        {
            return new DerivedTimer(observable.AsLog());
        }

        public Guid TimerId => _durationLog.LogId;

        public IDisposable Subscribe(IObserver<TimeSpan> observer)
        {
            return _durationLog.Subscribe(observer);
        }
    }
}
