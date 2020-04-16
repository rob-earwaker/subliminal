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
        private ThreadPoolMonitor(ILog<ThreadPoolUsage> threadPoolUsage)
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
                .AsLog());
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
        public ILog<ThreadPoolUsage> ThreadPoolUsage { get; }

        /// <summary>
        /// A gauge representing the current minimum number of worker threads in the pool.
        /// </summary>
        public IGauge MinWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => (double) usage.MinWorkerThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current maximum number of worker threads in the pool.
        /// </summary>
        public IGauge MaxWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => (double) usage.MaxWorkerThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current number of available worker threads in the pool.
        /// </summary>
        public IGauge AvailableWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => (double) usage.AvailableWorkerThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current number of active worker threads in the pool.
        /// </summary>
        public IGauge ActiveWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => (double) usage.ActiveWorkerThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current minimum number of completion port threads in the pool.
        /// </summary>
        public IGauge MinCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => (double) usage.MinCompletionPortThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current maximum number of completion port threads in the pool.
        /// </summary>
        public IGauge MaxCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => (double) usage.MaxCompletionPortThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current number of available completion port threads in the pool.
        /// </summary>
        public IGauge AvailableCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => (double) usage.AvailableCompletionPortThreads).AsGauge(); }
        }

        /// <summary>
        /// A gauge representing the current number of active completion port threads in the pool.
        /// </summary>
        public IGauge ActiveCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => (double) usage.ActiveCompletionPortThreads).AsGauge(); }
        }
    }
}
