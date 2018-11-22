using System;
using System.Collections.Generic;

namespace Lognostics
{
    public interface IScopeSource
    {
        Guid ScopeSourceId { get; }
        ICollection<IScope> ActiveScopes { get; }
    }
}
