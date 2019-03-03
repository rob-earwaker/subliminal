namespace Subliminal
{
    public interface IEventLog<TEvent> : ILog<TEvent>
    {
        ICounter Counter { get; }
    }
}
