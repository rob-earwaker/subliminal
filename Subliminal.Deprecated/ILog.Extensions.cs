using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class ILogExtensions
    {
        public static IObservable<TLogEntry> AsObservable<TLogEntry>(this ILog<TLogEntry> log)
            where TLogEntry : ILogEntry
        {
            return ObservableLog<TLogEntry>.FromLog(log);
        }

        public static ILog<IList<TLogEntry>> Buffer<TLogEntry>(this ILog<TLogEntry> log, int count, int skip)
            where TLogEntry : ILogEntry
        {
            var y = log.AsObservable().GroupBy(x => x.Context).SelectMany(g => g.Buffer(10).Select(b => (g.Key, b)));
            return y.Buffer(count, skip).AsLog();
        }

        public static ILog<TLogEntryResult> Select<TLogEntrySource, TLogEntryResult>(
            this ILog<TLogEntrySource> log, Func<TLogEntrySource, TLogEntryResult> selector)
            where TLogEntrySource : ILogEntry where TLogEntryResult : ILogEntry
        {
            return log.AsObservable().Select(selector).AsLog();
        }

        public static IDisposable Subscribe<TLogEntry>(this ILog<TLogEntry> log, Action<TLogEntry> handle)
            where TLogEntry : ILogEntry
        {
            return log.Subscribe(new LogHandler<TLogEntry>(handle));
        }
    }
}
