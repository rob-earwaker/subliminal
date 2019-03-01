using System;

namespace Subliminal
{
    public class DiscreteGauge : ISink<int>
    {
        private readonly Sink<int> _sink;

        public DiscreteGauge()
        {
            _sink = new Sink<int>();
        }

        public ISource<int> AsSource()
        {
            return _sink.AsSource();
        }

        public void OnCompleted()
        {
            _sink.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _sink.OnError(error);
        }

        public void OnNext(int value)
        {
            _sink.OnNext(value);
        }
    }
}
