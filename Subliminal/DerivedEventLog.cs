using System;
using System.Reactive;

namespace Subliminal
{
    public class DerivedEventLog<TContext> : IEventLog<TContext>
    {
        private readonly IObservable<TContext> _eventLogged;

        private DerivedEventLog(IObservable<TContext> eventLogged)
        {
            _eventLogged = eventLogged;
        }

        public static DerivedEventLog<TContext> FromLog(ILog<TContext> log)
        {
            return new DerivedEventLog<TContext>(log);
        }

        public static DerivedEventLog<TContext> FromObservable(IObservable<TContext> observable)
        {
            return FromLog(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<TContext> observer)
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
