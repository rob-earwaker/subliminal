using System;

namespace Lognostics.Events
{
    public class ScopeEnded : EventArgs
    {
        public ScopeEnded(IScope scope)
        {
            Scope = scope;
        }

        public IScope Scope { get; }
    }
}
