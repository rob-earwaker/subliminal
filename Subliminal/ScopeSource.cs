using Subliminal.Events;
using System.Collections.Generic;

namespace Subliminal
{
    public class ScopeSource : IScopeSource
    {
        private readonly ConcurrentHashSet<IScope> _activeScopes;

        public ScopeSource()
        {
            _activeScopes = new ConcurrentHashSet<IScope>();
        }

        public ICollection<IScope> ActiveScopes => _activeScopes.Snapshot();

        public IScope StartNew()
        {
            var scope = Scope.StartNew();
            scope.Ended += ScopeEndedHandler;
            _activeScopes.Add(scope);
            return scope;
        }

        private void ScopeEndedHandler(object sender, ScopeEnded eventArgs)
        {
            eventArgs.Scope.Ended -= ScopeEndedHandler;
        }
    }
}
