using System;
using System.Reactive.Linq;
using System.Threading;

namespace Subliminal
{
    public class ThreadPoolMonitor
    {
        private ThreadPoolMonitor(IMetric<ThreadPoolUsage> threadPoolUsage)
        {
            ThreadPoolUsage = threadPoolUsage;
        }

        public static ThreadPoolMonitor ForManagedThreadPool(TimeSpan samplingFrequency)
        {
            return new ThreadPoolMonitor(Observable
                .Interval(samplingFrequency)
                .Select(_ => GetManagedThreadPoolUsage())
                .AsMetric());
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

        public IMetric<ThreadPoolUsage> ThreadPoolUsage { get; }

        public IMetric<int> MinWorkerThreads
        {
            get
            {
                return ThreadPoolUsage
                    .Select(threadPoolUsage => threadPoolUsage.MinWorkerThreads)
                    .AsMetric();
            }
        }

        public IMetric<int> MinCompletionPortThreads
        {
            get
            {
                return ThreadPoolUsage
                    .Select(threadPoolUsage => threadPoolUsage.MinCompletionPortThreads)
                    .AsMetric();
            }
        }

        public IMetric<int> MaxWorkerThreads
        {
            get
            {
                return ThreadPoolUsage
                    .Select(threadPoolUsage => threadPoolUsage.MaxWorkerThreads)
                    .AsMetric();
            }
        }

        public IMetric<int> MaxCompletionPortThreads
        {
            get
            {
                return ThreadPoolUsage
                    .Select(threadPoolUsage => threadPoolUsage.MaxCompletionPortThreads)
                    .AsMetric();
            }
        }

        public IMetric<int> AvailableWorkerThreads
        {
            get
            {
                return ThreadPoolUsage
                    .Select(threadPoolUsage => threadPoolUsage.AvailableWorkerThreads)
                    .AsMetric();
            }
        }

        public IMetric<int> AvailableCompletionPortThreads
        {
            get
            {
                return ThreadPoolUsage
                    .Select(threadPoolUsage => threadPoolUsage.AvailableCompletionPortThreads)
                    .AsMetric();
            }
        }

        public IMetric<int> ActiveWorkerThreads
        {
            get
            {
                return ThreadPoolUsage
                    .Select(threadPoolUsage => threadPoolUsage.ActiveWorkerThreads)
                    .AsMetric();
            }
        }

        public IMetric<int> ActiveCompletionPortThreads
        {
            get
            {
                return ThreadPoolUsage
                    .Select(threadPoolUsage => threadPoolUsage.ActiveCompletionPortThreads)
                    .AsMetric();
            }
        }
    }
}
