using System;

namespace Subliminal
{
    /// <summary>
    /// A counter that both captures and emits increments.
    /// </summary>
    public sealed class Counter<TIncrement> : ICounter<TIncrement>
    {
        private readonly Log<TIncrement> _incrementLog;
        private readonly ICounter<TIncrement> _derivedCounter;

        /// <summary>
        /// Creates a counter that both captures and emits increments.
        /// </summary>
        public Counter()
        {
            _incrementLog = new Log<TIncrement>();
            _derivedCounter = _incrementLog.AsCounter();
        }

        /// <summary>
        /// Captures an increment and emits it to all observers.
        /// </summary>
        public void IncrementBy(TIncrement increment)
        {
            _incrementLog.Append(increment);
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future increments emitted
        /// by the counter. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<TIncrement> observer)
        {
            return _derivedCounter.Subscribe(observer);
        }
    }
}
