using System;
using System.Reactive.Linq;
using System.Threading;

namespace Subliminal
{
    public sealed class ThreadPoolMonitor
    {
        private ThreadPoolMonitor(IGauge<ThreadPoolUsage> threadPoolUsage)
        {
            ThreadPoolUsage = threadPoolUsage;
        }

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

        public IGauge<ThreadPoolUsage> ThreadPoolUsage { get; }

        public IGauge<int> MinWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.MinWorkerThreads).AsGauge(); }
        }

        public IGauge<int> MaxWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.MaxWorkerThreads).AsGauge(); }
        }

        public IGauge<int> AvailableWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.AvailableWorkerThreads).AsGauge(); }
        }

        public IGauge<int> ActiveWorkerThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.ActiveWorkerThreads).AsGauge(); }
        }

        public IGauge<int> MinCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.MinCompletionPortThreads).AsGauge(); }
        }

        public IGauge<int> MaxCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.MaxCompletionPortThreads).AsGauge(); }
        }

        public IGauge<int> AvailableCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.AvailableCompletionPortThreads).AsGauge(); }
        }

        public IGauge<int> ActiveCompletionPortThreads
        {
            get { return ThreadPoolUsage.Select(usage => usage.ActiveCompletionPortThreads).AsGauge(); }
        }
    }
}
