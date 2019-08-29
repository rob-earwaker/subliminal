using System;

namespace Subliminal
{
    /// <summary>
    /// A gauge that is derived from an observable source.
    /// </summary>
    public sealed class DerivedGauge<TValue> : IGauge<TValue>
    {
        private readonly ILog<TValue> _valueLog;

        private DerivedGauge(ILog<TValue> valueLog)
        {
            _valueLog = valueLog;
        }

        /// <summary>
        /// Creates a gauge from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static DerivedGauge<TValue> FromObservable(IObservable<TValue> observable)
        {
            return new DerivedGauge<TValue>(observable.AsLog());
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future values emitted
        /// by the gauge. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _valueLog.Subscribe(observer);
        }
    }
}
