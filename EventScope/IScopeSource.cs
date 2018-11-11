namespace EventScope
{
    public interface IScopeSource : IEventSource<ScopeStartedEventArgs>, ISubscription
    {
    }
}
