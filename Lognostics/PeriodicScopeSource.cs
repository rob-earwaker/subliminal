using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lognostics
{
    public class PeriodicScopeSource : IScopeSource
    {
        private readonly TimeSpan _scopeDuration;
        private readonly object _isStartedLock;
        private readonly object _activeScopeLock;
        private bool _isStarted;
        private IScope _activeScope;

        public PeriodicScopeSource(TimeSpan scopeDuration)
        {
            _scopeDuration = scopeDuration;
            _isStartedLock = new object();
            _activeScopeLock = new object();
            _isStarted = false;
            _activeScope = null;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            lock (_isStartedLock)
            {
                if (_isStarted)
                    return;

                _isStarted = true;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                lock (_activeScopeLock)
                {
                    _activeScope?.Stop();
                    _activeScope = Scope.StartNew();
                }

                try
                {
                    await Task.Delay(_scopeDuration, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }
            }

            _activeScope?.Stop();
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
    }
}
