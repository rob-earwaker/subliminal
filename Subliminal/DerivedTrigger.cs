using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedTrigger<TEvent> : ITrigger<TEvent>
    {
        private readonly IObservable<TEvent> _event;

        private DerivedTrigger(IObservable<TEvent> @event)
        {
            _event = @event;
            TriggerId = Guid.NewGuid();
        }

        public static DerivedTrigger<TEvent> FromObservable(IObservable<TEvent> events)
        {
            return new DerivedTrigger<TEvent>(events.Take(1));
        }

        public Guid TriggerId { get; }

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _event.Subscribe(observer);
        }
    }
}
