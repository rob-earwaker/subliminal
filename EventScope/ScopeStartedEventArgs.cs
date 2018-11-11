using System;

namespace EventScope
{
    public class ScopeStartedEventArgs : ScopedEventArgs
    {
        public ScopeStartedEventArgs(IScope startedScope) : base(startedScope.ParentScope)
        {
            StartedScope = startedScope;
        }

        public IScope StartedScope { get; }
    }
}