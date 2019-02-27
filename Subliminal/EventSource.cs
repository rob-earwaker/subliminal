using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class EventSource<TEvent> : IEventSource<TEvent>
    {
        private EventSource(IObservable<Event<TEvent>> source)
        {
            Source = source.Publish().AutoConnect();
        }

        public static EventSource<TEvent> FromSource(IObservable<TEvent> source)
        {
            return new EventSource<TEvent>(source
                .Timestamp()
                .TimeInterval()
                .Select(x => new Event<TEvent>(x.Value.Value, x.Value.Timestamp, x.Interval)));
        }

        public IObservable<Event<TEvent>> Source { get; }
    }
}
