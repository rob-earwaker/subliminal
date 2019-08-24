using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedEventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly ILog<TEvent> _eventLog;

        private DerivedEventLog(ILog<TEvent> eventLog)
        {
            _eventLog = eventLog;
        }

        public ICounter<long> EventCounter => _eventLog.Select(_ => 1L).AsCounter();

        public static DerivedEventLog<TEvent> FromObservable(IObservable<TEvent> observable)
        {
            return new DerivedEventLog<TEvent>(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }

    public class DerivedEventLog : IEventLog
    {
        private readonly IEventLog<Unit> _eventLog;

        private DerivedEventLog(IEventLog<Unit> eventLog)
        {
            _eventLog = eventLog;
        }

        public ICounter<long> EventCounter => _eventLog.EventCounter;

        public static DerivedEventLog FromObservable(IObservable<Unit> observable)
        {
            return new DerivedEventLog(observable.AsEventLog<Unit>());
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
