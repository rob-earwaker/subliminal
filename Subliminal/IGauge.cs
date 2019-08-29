namespace Subliminal
{
    /// <summary>
    /// An observable log of values.
    /// </summary>
    public interface IGauge<out TValue> : ILog<TValue>
    {
    }
}
