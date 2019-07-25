﻿using System;

namespace Subliminal
{
    public class DerivedTimer : ITimer
    {
        private readonly ILog<TimeSpan> _durationLog;

        private DerivedTimer(ILog<TimeSpan> durationLog)
        {
            _durationLog = durationLog;
        }

        public static DerivedTimer FromObservable(IObservable<TimeSpan> durations)
        {
            return new DerivedTimer(durations.AsLog());
        }

        public Guid TimerId => _durationLog.LogId;

        public IDisposable Subscribe(IObserver<TimeSpan> observer)
        {
            return _durationLog.Entries.Subscribe(observer);
        }
    }
}
