namespace Subliminal
{
    public interface ITimingEvent<TTimingEvent> : ITimingEventLog<TTimingEvent>
        where TTimingEvent : ITiming
    {
    }
}
