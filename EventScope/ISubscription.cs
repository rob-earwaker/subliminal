using System.Collections.Generic;

namespace EventScope
{
    public interface ISubscription : IEventHandler<ScopeStartedEventArgs>
    {
        bool Active { get; }
        HashSet<IScope> ActiveScopes { get; }
    }
}
