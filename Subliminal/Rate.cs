﻿using System;
using System.Reactive;

namespace Subliminal
{
    public class Rate<TDelta>
    {
        public Rate(TDelta delta, TimeSpan interval)
        {
            Delta = delta;
            Interval = interval;
        }

        public TDelta Delta { get; }
        public TimeSpan Interval { get; }
    }
}