namespace Subliminal
{
    public class TimerStarted
    {
        public TimerStarted(ITimingEvent<TimerEnded> ended)
        {
            Ended = ended;
        }
        
        public ITimingEvent<TimerEnded> Ended { get; }
    }
}
