using System;

namespace Lognostics
{
    public interface IEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        void HandleEvent(object sender, TEventArgs eventArgs);
    }
}
