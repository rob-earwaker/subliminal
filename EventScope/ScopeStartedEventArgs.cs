namespace EventScope
{
    public class ScopeStartedEventArgs : ScopedEventArgs
    {
        public ScopeStartedEventArgs(IScope scope) : base(scope)
        {
        }
    }
}