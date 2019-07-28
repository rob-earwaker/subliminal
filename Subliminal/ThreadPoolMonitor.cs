using System;
using System.Reactive.Linq;
using System.Threading;

namespace Subliminal
{
    public class ThreadPoolMonitor
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

        public IGauge<ThreadPoolUsage> ThreadPoolUsage { get; }

        public IGauge<int> MinWorkerThreads
        {
            get
            {
                return ThreadPoolUsage.Values
                    .Select(threadPoolUsage => threadPoolUsage.MinWorkerThreads)
                    .AsMetric();
            }
        }

        public IGauge<int> MinCompletionPortThreads
        {
            get
            {
                return ThreadPoolUsage.Values
                    .Select(threadPoolUsage => threadPoolUsage.MinCompletionPortThreads)
                    .AsMetric();
            }
        }

        public IGauge<int> MaxWorkerThreads
        {
            get
            {
                return ThreadPoolUsage.Values
                    .Select(threadPoolUsage => threadPoolUsage.MaxWorkerThreads)
                    .AsMetric();
            }
        }

        public IGauge<int> MaxCompletionPortThreads
        {
            get
            {
                return ThreadPoolUsage.Values
                    .Select(threadPoolUsage => threadPoolUsage.MaxCompletionPortThreads)
                    .AsMetric();
            }
        }

        public IGauge<int> AvailableWorkerThreads
        {
            get
            {
                return ThreadPoolUsage.Values
                    .Select(threadPoolUsage => threadPoolUsage.AvailableWorkerThreads)
                    .AsMetric();
            }
        }

        public IGauge<int> AvailableCompletionPortThreads
        {
            get
            {
                return ThreadPoolUsage.Values
                    .Select(threadPoolUsage => threadPoolUsage.AvailableCompletionPortThreads)
                    .AsMetric();
            }
        }

        public IGauge<int> ActiveWorkerThreads
        {
            get
            {
                return ThreadPoolUsage.Values
                    .Select(threadPoolUsage => threadPoolUsage.ActiveWorkerThreads)
                    .AsMetric();
            }
        }

        public IGauge<int> ActiveCompletionPortThreads
        {
            get
            {
                return ThreadPoolUsage.Values
                    .Select(threadPoolUsage => threadPoolUsage.ActiveCompletionPortThreads)
                    .AsMetric();
            }
        }
    }
}
