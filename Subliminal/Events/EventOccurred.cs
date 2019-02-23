namespace Subliminal.Events
{
    public class EventOccurred<TEvent>
    {
        public EventOccurred(TEvent @event)
        {
            Event = @event;
        }

        public TEvent Event { get; }
    }
}
