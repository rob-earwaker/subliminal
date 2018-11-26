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
            _scopeSources = scopeSources;
        }

        public ICollection<IScope> ActiveScopes => _scopeSources.SelectMany(scopeSource => scopeSource.ActiveScopes).ToArray();
    }
}
