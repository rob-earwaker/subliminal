using System;
using System.Collections.Generic;
using System.Linq;

namespace Lognostics
{
    public class AggregateScopeSource : IScopeSource
    {
        private readonly IScopeSource[] _scopeSources;

        public AggregateScopeSource(params IScopeSource[] scopeSources)
        {
            ScopeSourceId = Guid.NewGuid();
            _scopeSources = scopeSources;
        }

        public Guid ScopeSourceId { get; }
        public ICollection<IScope> ActiveScopes => _scopeSources.SelectMany(scopeSource => scopeSource.ActiveScopes).ToArray();
    }
}
