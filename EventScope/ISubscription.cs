using System.Collections.Generic;

namespace EventScope
{
    public interface ISubscription : IEventHandler<ScopeStartedEventArgs>
    {
        bool IsActive { get; }
        HashSet<IScope> ActiveScopes { get; }
    }
}
