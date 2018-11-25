using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace Lognostics.Serilog
{
    public class DictionaryEnricher : ILogEventEnricher
    {
        private readonly IReadOnlyDictionary<string, object> _dictionary;

        public DictionaryEnricher(IReadOnlyDictionary<string, object> dictionary)
        {
            _dictionary = dictionary;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var logEventEnrichers = _dictionary
                .Select(context => new PropertyEnricher(context.Key, context.Value, destructureObjects: true))
                .ToArray();

            foreach (var logEventEnricher in logEventEnrichers)
            {
                logEventEnricher.Enrich(logEvent, propertyFactory);
            }
        }
    }
}
