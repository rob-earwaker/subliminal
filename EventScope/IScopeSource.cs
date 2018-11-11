namespace EventScope
{
    public interface IScopeSource : ISubscription
    {
        void AddHandler(ISubscription subscription);
        void RemoveHandler(ISubscription subscription);
    }
}
