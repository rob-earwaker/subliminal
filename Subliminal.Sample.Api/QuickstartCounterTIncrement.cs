﻿using System;

namespace Subliminal.Sample.Api
{
    internal class QuickstartCounterTIncrement : ISample
    {
        public string SampleName => "Quickstart: Counter<TIncrement>";

        public void RunSample()
        {
            var entitiesRetrieved = new Counter<long>();

            entitiesRetrieved.Subscribe(entityCount =>
                Console.WriteLine($"Retrieved another {entityCount} entities"));

            var entityPages = new[]
            {
                new { EntityCount = 100 },
                new { EntityCount = 100 },
                new { EntityCount = 46 }
            };

            foreach (var entityPage in entityPages)
            {
                entitiesRetrieved.IncrementBy(entityPage.EntityCount);
            }
            // "Retrieved another 100 entities"
            // "Retrieved another 100 entities"
            // "Retrieved another 46 entities"
        }
    }
}
