namespace EventScope
{
    public class ScopeEndedEventArgs : ScopedEventArgs
    {
        public ScopeEndedEventArgs(IScope endedScope) : base(endedScope.ParentScope)
        {
            EndedScope = endedScope;
        }

        public IScope EndedScope { get; }
    }
}
