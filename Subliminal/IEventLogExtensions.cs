using System.Reactive.Linq;

namespace Subliminal
{
    public static class IEventLogExtensions
    {
        public static ICounter<long> Counter<TContext>(this IEventLog<TContext> eventLog)
        {
            return eventLog.Select(context => 1L).AsCounter();
        }
    }
}
