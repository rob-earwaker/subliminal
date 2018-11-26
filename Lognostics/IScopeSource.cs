using System;
using System.Collections.Generic;

namespace Lognostics
{
    public interface IScopeSource
    {
        ICollection<IScope> ActiveScopes { get; }
    }
}
