using System;

namespace Lognostics
{
    public class ScopeEndedEventArgs : EventArgs
    {
        public ScopeEndedEventArgs(IScope scope)
        {
            Scope = scope;
        }

        public IScope Scope { get; }
    }
}
