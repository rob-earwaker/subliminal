using System;
using System.Reactive.Linq;
using System.Threading;

namespace Subliminal
{
    /// <summary>
    /// A monitor that provides information about the state of a thread pool.
    /// </summary>
    public sealed class ThreadPoolMonitor
    {
        private ThreadPoolMonitor(IGauge<ThreadPoolUsage> threadPoolUsage)
        {
            ThreadPoolUsage = threadPoolUsage;
        }

        /// <summary>
        /// Creates a monitor for the managed thread pool that emits information according to
        /// the sampling frequency.
        /// </summary>
        public static ThreadPoolMonitor ForManagedThreadPool(TimeSpan samplingFrequency)
        {
            return new ThreadPoolMonitor(Observable
                .Interval(samplingFrequency)
                .Select(_ => GetManagedThreadPoolUsage())
                .AsGauge());
        }

        private static ThreadPoolUsage GetManagedThreadPoolUsage()
        {
            ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
            ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
            ThreadPool.GetAvailableThreads(out var availableWorkerThreads, out var availableCompletionPortThreads);

            return new ThreadPoolUsage(
                minWorkerThreads, minCompletionPortThreads,
                maxWorkerThreads, maxCompletionPortThreads,
                availableWorkerThreads, availableCompletionPortThreads);
        }

        /// <summary>
        /// A gauge representing the current thread pool usage.
        /// </summary>
        public IGauge<ThreadPoolUsage> ThreadPoolUsage { get; }

        /// <summary>
        /// A gauge representing the current minimum number of worker threads in the pool.
        /// </summary>
        public IGauge<int> MinWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.MinWorkerThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current maximum number of worker threads in the pool.
        /// </summary>
        public IGauge<int> MaxWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.MaxWorkerThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current number of available worker threads in the pool.
        /// </summary>
        public IGauge<int> AvailableWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.AvailableWorkerThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current number of active worker threads in the pool.
        /// </summary>
        public IGauge<int> ActiveWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.ActiveWorkerThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current minimum number of completion port threads in the pool.
        /// </summary>
        public IGauge<int> MinCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.MinCompletionPortThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current maximum number of completion port threads in the pool.
        /// </summary>
        public IGauge<int> MaxCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.MaxCompletionPortThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current number of available completion port threads in the pool.
        /// </summary>
        public IGauge<int> AvailableCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.AvailableCompletionPortThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current number of active completion port threads in the pool.
        /// </summary>
        public IGauge<int> ActiveCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.ActiveCompletionPortThreads).AsGauge(); }
        }
    }
}
