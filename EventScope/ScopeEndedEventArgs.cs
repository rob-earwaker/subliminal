namespace EventScope
{
    public class ScopeEndedEventArgs : ScopedEventArgs
    {
        public ScopeEndedEventArgs(IScope scope) : base(scope)
        {
        }
    }
}
