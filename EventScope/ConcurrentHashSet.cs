﻿using System.Collections.Generic;
using System.Linq;

namespace EventScope
{
    public class ConcurrentHashSet<TValue>
    {
        private readonly HashSet<TValue> _hashSet;
        private readonly object _hashSetLock;

        public ConcurrentHashSet()
        {
            _hashSet = new HashSet<TValue>();
            _hashSetLock = new object();
        }

        public void Add(TValue value)
        {
            lock (_hashSetLock)
            {
                _hashSet.Add(value);
            }
        }

        public void Remove(TValue value)
        {
            lock (_hashSetLock)
            {
                _hashSet.Remove(value);
            }
        }

        public HashSet<TValue> Snapshot()
        {
            lock (_hashSetLock)
            {
                return new HashSet<TValue>(_hashSet);
            }
        }

        public bool Any()
        {
            lock (_hashSetLock)
            {
                return _hashSet.Any();
            }
        }
    }
}
