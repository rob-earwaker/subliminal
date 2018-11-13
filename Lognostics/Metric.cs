using System;

namespace Lognostics
{
    public class Metric<TMetric> : IEventSource<MetricSampledEventArgs<TMetric>>
    {
        public event EventHandler<MetricSampledEventArgs<TMetric>> Sampled;

        public void LogValue(TMetric value)
        {
            Sampled?.Invoke(this, new MetricSampledEventArgs<TMetric>(value));
        }

        public void AddHandler(IEventHandler<MetricSampledEventArgs<TMetric>> eventHandler)
        {
            AddHandler(eventHandler.HandleEvent);
        }

        public void AddHandler(EventHandler<MetricSampledEventArgs<TMetric>> eventHandler)
        {
            Sampled += eventHandler;
        }

        public void RemoveHandler(IEventHandler<MetricSampledEventArgs<TMetric>> eventHandler)
        {
            RemoveHandler(eventHandler.HandleEvent);
        }

        public void RemoveHandler(EventHandler<MetricSampledEventArgs<TMetric>> eventHandler)
        {
            Sampled -= eventHandler;
        }
    }
}
