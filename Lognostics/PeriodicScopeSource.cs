using System;
using System.Collections.Generic;
using System.Timers;

namespace Lognostics
{
    public class PeriodicScopeSource : IScopeSource, IDisposable
    {
        private readonly TimeSpan _scopeDuration;
        private readonly Timer _timer;
        private readonly object _timerLock;
        private IScope _activeScope;
        private readonly object _activeScopeLock;

        public PeriodicScopeSource(TimeSpan scopeDuration)
        {
            _scopeDuration = scopeDuration;
            _timer = new Timer(scopeDuration.TotalMilliseconds) { AutoReset = true };
            _timerLock = new object();
            _activeScope = null;
            _activeScopeLock = new object();
        }

        public static PeriodicScopeSource StartNew(TimeSpan scopeDuration)
        {
            var periodicScopeSource = new PeriodicScopeSource(scopeDuration);
            periodicScopeSource.Start();
            return periodicScopeSource;
        }

        public void Start()
        {
            lock (_timerLock)
            {
                if (_timer.Enabled)
                    return;

                _timer.Elapsed += RenewActiveScope;
                _timer.Disposed += EndActiveScope;
                _timer.Start();
            }
        }

        private void RenewActiveScope(object sender, EventArgs eventArgs)
        {
            lock (_activeScopeLock)
            {
                _activeScope?.End();
                _activeScope = Scope.StartNew();
            }
        }

        private void EndActiveScope(object sender, EventArgs eventArgs)
        {
            _timer.Elapsed -= RenewActiveScope;
            _timer.Disposed -= EndActiveScope;

            lock (_activeScopeLock)
            {
                _activeScope?.End();
                _activeScope = null;
            }
        }

        public ICollection<IScope> ActiveScopes
        {
            get
            {
                lock (_activeScopeLock)
                {
                    return _activeScope == null ? new IScope[0] : new[] { _activeScope };
                }
            }
        }

        public void Dispose()
        {
            lock (_timerLock)
            {
                _timer.Dispose();
            }
        }
    }
}
