using System;
using System.Collections.Generic;

namespace Subliminal
{
    public interface IScopeSource
    {
        ICollection<IScope> ActiveScopes { get; }
    }
}
