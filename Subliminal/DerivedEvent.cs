using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedEvent<TEvent> : IEvent<TEvent>
    {
        private readonly IEventLog<TEvent> _eventLog;

        private DerivedEvent(IEventLog<TEvent> eventLog)
        {
            _eventLog = eventLog;
        }

        public static DerivedEvent<TEvent> FromObservable(IObservable<TEvent> observable)
        {
            return new DerivedEvent<TEvent>(observable.Take(1).AsEventLog());
        }

        public ICounter Counter => _eventLog.Counter;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _eventLog.Subscribe(observer);
        }
    }
}
