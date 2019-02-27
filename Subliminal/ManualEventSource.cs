using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class ManualEventSource<TEvent> : IEventSource<TEvent>
    {
        private readonly Subject<TEvent> _source;
        private readonly IEventSource<TEvent> _eventSource;

        public ManualEventSource()
        {
            _source = new Subject<TEvent>();
            _eventSource = _source.AsObservable().AsEventSource();
        }

        public IObservable<Event<TEvent>> Source => _eventSource.Source;

        public void LogOccurrence(TEvent @event)
        {
            _source.OnNext(@event);
        }
    }
}
