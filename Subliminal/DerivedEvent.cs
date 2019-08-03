using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedEvent<TContext> : IEvent<TContext>
    {
        private readonly IObservable<TContext> _raised;

        private DerivedEvent(IObservable<TContext> raised)
        {
            _raised = raised;
        }

        public static DerivedEvent<TContext> FromObservable(IObservable<TContext> observable)
        {
            // Take a single context value from the observable to ensure that the
            // event is only raised once, and replay this value to all observers.
            var raised = observable.Take(1).Replay();

            // Connect immediately so that observers can consume the event.
            raised.Connect();

            return new DerivedEvent<TContext>(raised);
        }

        public IDisposable Subscribe(IObserver<TContext> observer)
        {
            return _raised.Subscribe(observer);
        }
    }

    public class DerivedEvent : IEvent
    {
        private readonly IEventLog<Unit> _eventLog;

        private DerivedEvent(IEventLog<Unit> eventLog)
        {
            _eventLog = eventLog;
        }

        public static DerivedEvent FromLog(ILog<Unit> log)
        {
            return new DerivedEvent(log.AsEventLog());
        }

        public static DerivedEvent FromObservable(IObservable<Unit> observable)
        {
            return FromLog(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
