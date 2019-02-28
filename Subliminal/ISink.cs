using System;

namespace Subliminal
{
    public interface ISink<TValue> : IObserver<TValue>
    {
        ISource<TValue> AsSource();
    }
}
