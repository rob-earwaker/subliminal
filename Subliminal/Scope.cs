﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Scope : IScope
    {
        private readonly Stopwatch _stopwatch;
        private readonly ConcurrentDictionary<string, object> _context;
        private readonly Subject<Unit> _ended;

        public Scope()
        {
            HasStarted = false;
            HasEnded = false;

            _stopwatch = new Stopwatch();
            _context = new ConcurrentDictionary<string, object>();
            _ended = new Subject<Unit>();
        }

        public static Scope StartNew()
        {
            var scope = new Scope();
            scope.Start();
            return scope;
        }

        public bool HasStarted { get; private set; }
        public IReadOnlyDictionary<string, object> Context => _context;
        public TimeSpan Duration => _stopwatch.Elapsed;
        public bool HasEnded { get; private set; }

        public IObservable<Unit> Ended => _ended.AsObservable();

        public void Start()
        {
            if (HasStarted)
                return;

            _stopwatch.Start();

            HasStarted = true;
        }

        public void End()
        {
            if (!HasStarted || HasEnded)
                return;

            _stopwatch.Stop();

            _ended.OnNext(Unit.Default);
            _ended.OnCompleted();

            HasEnded = true;
        }

        public void AddContext(string contextKey, object value)
        {
            _context.AddOrUpdate(contextKey, value, (_, __) => value);
        }

        public void Dispose()
        {
            End();
        }
    }
}
