using System;

namespace EventScope
{
    public class ScopeStartedEventArgs : EventArgs
    {
        public ScopeStartedEventArgs(IScope scope)
        {
            Scope = scope;
        }

        public IScope Scope { get; }
    }
}