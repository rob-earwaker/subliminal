using System;

namespace Subliminal
{
    /// <summary>
    /// A counter that is derived from an observable source.
    /// </summary>
    public sealed class DerivedCounter<TIncrement> : ICounter<TIncrement>
    {
        private readonly ILog<TIncrement> _incrementLog;

        private DerivedCounter(ILog<TIncrement> incrementLog)
        {
            _incrementLog = incrementLog;
        }

        /// <summary>
        /// Creates a counter from an observable source. This creates a subscription
        /// to the source observable that will start consuming items immediately.
        /// </summary>
        public static DerivedCounter<TIncrement> FromObservable(IObservable<TIncrement> observable)
        {
            return new DerivedCounter<TIncrement>(observable.AsLog());
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future increments emitted
        /// by the counter. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TIncrement> observer)
        {
            return _incrementLog.Subscribe(observer);
        }
    }
}
