using Subliminal.Events;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Event<TEvent>
    {
        private readonly Subject<EventOccurred<TEvent>> _occurred;

        public Event()
        {
            _occurred = new Subject<EventOccurred<TEvent>>();
        }

        public IObservable<EventOccurred<TEvent>> Occurred => _occurred.AsObservable();

        public void LogOccurrence(TEvent @event)
        {
            _occurred.OnNext(new EventOccurred<TEvent>(@event));
        }
    }
}
