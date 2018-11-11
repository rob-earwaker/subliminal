using System.Collections.Generic;

namespace EventScope
{
    public interface ISubscription
    {
        bool IsActive { get; }
        HashSet<IScope> ActiveScopes { get; }
        void HandleEvent(object sender, ScopeStartedEventArgs eventArgs);
    }
}
