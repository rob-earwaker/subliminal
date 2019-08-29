namespace Subliminal
{
    /// <summary>
    /// A metric containing information about thread pool usage.
    /// </summary>
    public sealed class ThreadPoolUsage
    {
        /// <summary>
        /// Creates a metric containing information about thread pool usage.
        /// </summary>
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

        /// <summary>
        /// The current minimum number of worker threads in the pool.
        /// </summary>
        public int MinWorkerThreads { get; }

        /// <summary>
        /// The current minimum number of completion port threads in the pool.
        /// </summary>
        public int MinCompletionPortThreads { get; }

        /// <summary>
        /// The current maximum number of worker threads in the pool.
        /// </summary>
        public int MaxWorkerThreads { get; }

        /// <summary>
        /// The current maximum number of completion port threads in the pool.
        /// </summary>
        public int MaxCompletionPortThreads { get; }

        /// <summary>
        /// The current number of available worker threads in the pool.
        /// </summary>
        public int AvailableWorkerThreads { get; }

        /// <summary>
        /// The current number of available completion port threads in the pool.
        /// </summary>
        public int AvailableCompletionPortThreads { get; }

        /// <summary>
        /// The current number of active worker threads in the pool.
        /// </summary>
        public int ActiveWorkerThreads => MaxWorkerThreads - AvailableWorkerThreads;

        /// <summary>
        /// The current number of active completion port threads in the pool.
        /// </summary>
        public int ActiveCompletionPortThreads => MaxCompletionPortThreads - AvailableCompletionPortThreads;
    }
}
