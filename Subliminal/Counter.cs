using System;

namespace Subliminal
{
    /// <summary>
    /// A counter that both captures and emits increments.
    /// </summary>
    public sealed class Counter : ICounter
    {
        private readonly Log<double> _incrementLog;
        private readonly ICounter _derivedCounter;

        /// <summary>
        /// Creates a counter that both captures and emits increments.
        /// </summary>
        public Counter()
        {
            _incrementLog = new Log<double>();
            _derivedCounter = _incrementLog.AsCounter();
        }

        /// <summary>
        /// Captures an increment and emits it to all observers.
        /// </summary>
        public void IncrementBy(double increment)
        {
            _incrementLog.Append(increment);
        }

        /// <summary>
        /// Captures an increment of one and emits it to all observers.
        /// </summary>
        public void Increment()
        {
            IncrementBy(1);
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future increments emitted
        /// by the counter. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<double> observer)
        {
            return _derivedCounter.Subscribe(observer);
        }
    }
}
