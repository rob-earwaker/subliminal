using System;

namespace EventScope
{
    public class ScopeStoppedEventArgs : EventArgs
    {
        public ScopeStoppedEventArgs(IScope scope)
        {
            Scope = scope;
        }

        public IScope Scope { get; }
    }
}
