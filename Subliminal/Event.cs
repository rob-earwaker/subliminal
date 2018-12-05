using System;

namespace Subliminal
{
    public class Event<TEventArgs> where TEventArgs : EventArgs
    {
        public event EventHandler<TEventArgs> Occured;

        public void LogOccurrence(TEventArgs eventArgs)
        {
            Occured?.Invoke(this, eventArgs);
        }
    }
}
