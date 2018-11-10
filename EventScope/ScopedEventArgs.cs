using System;

namespace EventScope
{
    public class ScopedEventArgs : EventArgs
    {
        public ScopedEventArgs(IScope scope)
        {
            Scope = scope;
        }

        public IScope Scope { get; }
    }
}
