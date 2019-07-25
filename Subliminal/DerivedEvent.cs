using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedEvent<TEvent> : IEvent<TEvent>
    {
        private readonly IObservable<TEvent> _event;

        private DerivedEvent(IObservable<TEvent> @event)
        {
            _event = @event;
            EventId = Guid.NewGuid();
        }

        public static DerivedEvent<TEvent> FromObservable(IObservable<TEvent> events)
        {
            return new DerivedEvent<TEvent>(events.Take(1));
        }

        public Guid EventId { get; }

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _event.Subscribe(observer);
        }
    }
}
