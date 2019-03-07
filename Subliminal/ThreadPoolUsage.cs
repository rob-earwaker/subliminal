namespace Subliminal
{
    public class ThreadPoolUsage
    {
        public ThreadPoolUsage(
            int minWorkerThreads, int minCompletionPortThreads,
            int maxWorkerThreads, int maxCompletionPortThreads,
            int availableWorkerThreads, int availableCompletionPortThreads)
        {
            MinWorkerThreads = minWorkerThreads;
            MinCompletionPortThreads = minCompletionPortThreads;
            MaxWorkerThreads = maxWorkerThreads;
            MaxCompletionPortThreads = maxCompletionPortThreads;
            AvailableWorkerThreads = availableWorkerThreads;
            AvailableCompletionPortThreads = availableCompletionPortThreads;
        }

        public int MinWorkerThreads { get; }
        public int MinCompletionPortThreads { get; }
        public int MaxWorkerThreads { get; }
        public int MaxCompletionPortThreads { get; }
        public int AvailableWorkerThreads { get; }
        public int AvailableCompletionPortThreads { get; }
        public int ActiveWorkerThreads => MaxWorkerThreads - AvailableWorkerThreads;
        public int ActiveCompletionPortThreads => MaxCompletionPortThreads - AvailableCompletionPortThreads;
    }
}
