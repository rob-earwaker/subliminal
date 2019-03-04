namespace Subliminal
{
    public interface ITimingEventLog<TTimingEvent> : IEventLog<TTimingEvent>
        where TTimingEvent : ITiming
    {
        ITimer Timer { get; }
    }
}
