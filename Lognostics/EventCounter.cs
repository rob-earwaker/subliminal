using System;

namespace Lognostics
{
    public class EventCounter<TEventArgs> : IEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private int _eventCount;
        private readonly object _eventCountLock;

        public EventCounter()
        {
            _eventCount = 0;
            _eventCountLock = new object();
        }

        public int EventCount
        {
            get
            {
                lock (_eventCountLock)
                {
                    return _eventCount;
                }
            }
        }

        public void HandleEvent(object sender, TEventArgs eventArgs)
        {
            lock (_eventCountLock)
            {
                _eventCount++;
            }
        }
    }
}
