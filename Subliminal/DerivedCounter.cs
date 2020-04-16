using System;

namespace Subliminal
{
    /// <summary>
    /// A counter that is derived from an observable source.
    /// </summary>
    public sealed class DerivedCounter : ICounter
    {
        private readonly ILog<double> _incrementLog;

        private DerivedCounter(ILog<double> incrementLog)
        {
            _incrementLog = incrementLog;
        }

        /// <summary>
        /// Creates a counter from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static DerivedCounter FromObservable(IObservable<double> observable)
        {
            return new DerivedCounter(observable.AsLog());
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future increments emitted
        /// by the counter. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<double> observer)
        {
            return _incrementLog.Subscribe(observer);
        }
    }
}
