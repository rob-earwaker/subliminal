using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedEventLog<TEvent> : IEventLog<TEvent>
    {
        private readonly ILog<TEvent> _log;

        private DerivedEventLog(ILog<TEvent> log)
        {
            _log = log;
        }

        public static DerivedEventLog<TEvent> FromObservable(IObservable<TEvent> observable)
        {
            return new DerivedEventLog<TEvent>(observable.AsLog());
        }

        public ICounter Counter => _log.Select(_ => 1).AsCounter();

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
