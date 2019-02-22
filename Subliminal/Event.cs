using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Event<TEvent>
    {
        private readonly Subject<TEvent> _occurred;

        public Event()
        {
            _occurred = new Subject<TEvent>();
        }

        public IObservable<TEvent> Occured => _occurred.AsObservable();

        public void LogOccurrence(TEvent @event)
        {
            _occurred.OnNext(@event);
        }
    }
}
