using System.Collections.Generic;

namespace EventScope
{
    public interface IScopeSource
    {
        ICollection<IScope> ActiveScopes { get; }
    }
}
