using System;

namespace EventScope
{
    public class ScopedEventArgs : EventArgs
    {
        public ScopedEventArgs(IScope eventScope)
        {
            EventScope = eventScope;
        }

        public IScope EventScope { get; }
    }
}
