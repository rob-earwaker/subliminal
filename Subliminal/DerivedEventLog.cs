using System;
using System.Reactive;

namespace Subliminal
{
    public class DerivedEventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly IObservable<TEvent> _eventLogged;

        private DerivedEventLog(IObservable<TEvent> eventLogged)
        {
            _eventLogged = eventLogged;
        }

        public static DerivedEventLog<TEvent> FromLog(ILog<TEvent> log)
        {
            return new DerivedEventLog<TEvent>(log);
        }

        public static DerivedEventLog<TEvent> FromObservable(IObservable<TEvent> observable)
        {
            return FromLog(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _eventLogged.Subscribe(observer);
        }
    }

    public class DerivedEventLog : IEventLog
    {
        private readonly IEventLog<Unit> _eventLog;

        private DerivedEventLog(IEventLog<Unit> eventLog)
        {
            _eventLog = eventLog;
        }

        public static DerivedEventLog FromLog(ILog<Unit> log)
        {
            return new DerivedEventLog(log.AsEventLog());
        }

        public static DerivedEventLog FromObservable(IObservable<Unit> observable)
        {
            return FromLog(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
