namespace Subliminal
{
    public interface ITimingEvent<TTimingEvent> : IEvent<TTimingEvent>, ITimingEventLog<TTimingEvent>
        where TTimingEvent : ITiming
    {
    }
}
