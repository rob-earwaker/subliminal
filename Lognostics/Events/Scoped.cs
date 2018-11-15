using System;

namespace Lognostics.Events
{
    public class Scoped<TValue> : EventArgs
    {
        public Scoped(IScope scope, TValue value)
        {
            Scope = scope;
            Value = value;
        }

        public IScope Scope { get; }
        public TValue Value { get; }
    }
}
