namespace Subliminal
{
    /// <summary>
    /// An observable log of increments.
    /// </summary>
    public interface ICounter<out TIncrement> : ILog<TIncrement>
    {
    }
}
