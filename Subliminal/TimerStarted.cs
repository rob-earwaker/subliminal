using System;

namespace Subliminal
{
    public class TimerStarted
    {
        public TimerStarted(IEvent<TimerEnded> ended)
        {
            Ended = ended;
        }
        
        public IEvent<TimerEnded> Ended { get; }
    }
}
