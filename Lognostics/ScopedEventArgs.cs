using System;

namespace Lognostics
{
    public class ScopedEventArgs<TValue> : EventArgs
    {
        public ScopedEventArgs(IScope scope, TValue value)
        {
            Scope = scope;
            Value = value;
        }

        public IScope Scope { get; }
        public TValue Value { get; }
    }
}
