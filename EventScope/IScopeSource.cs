namespace EventScope
{
    public interface IScopeSource : ISubscription
    {
        void AddSubscription(ISubscription subscription);
        void RemoveSubscription(ISubscription subscription);
    }
}
