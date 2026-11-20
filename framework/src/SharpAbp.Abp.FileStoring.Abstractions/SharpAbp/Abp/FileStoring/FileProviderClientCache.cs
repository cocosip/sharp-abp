using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Caches long-lived SDK clients by a structural key. Clients are removed after an idle
    /// timeout and disposed after an additional grace period, so configuration changes do not
    /// leak clients forever and in-flight operations are not interrupted.
    /// </summary>
    public class FileProviderClientCache<TClient> : IDisposable
        where TClient : class
    {
        private static readonly TimeSpan DefaultIdleTimeout = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan DefaultGracePeriod = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan MaxSweepInterval = TimeSpan.FromMinutes(1);

        private sealed class Entry
        {
            public TClient Client { get; }

            private long _lastUsedTicks;
            private int _released;

            public Entry(TClient client)
            {
                Client = client;
                Touch();
            }

            public long LastUsedTicks => Interlocked.Read(ref _lastUsedTicks);

            public void Touch()
            {
                Interlocked.Exchange(ref _lastUsedTicks, DateTime.UtcNow.Ticks);
            }

            public bool TryRelease()
            {
                return Interlocked.Exchange(ref _released, 1) == 0;
            }
        }

        private sealed class RetiredClient
        {
            public TClient Client { get; }

            public long DisposeAtTicks { get; }

            public RetiredClient(TClient client, long disposeAtTicks)
            {
                Client = client;
                DisposeAtTicks = disposeAtTicks;
            }
        }

        private readonly ConcurrentDictionary<string, Lazy<Entry>> _syncEntries = new();
        private readonly ConcurrentDictionary<string, Lazy<Task<Entry>>> _asyncEntries = new();
        private readonly ConcurrentQueue<RetiredClient> _retiredClients = new();

        private readonly TimeSpan _idleTimeout;
        private readonly TimeSpan _gracePeriod;
        private readonly TimeSpan _sweepInterval;

        private long _nextSweepTicks;
        private int _disposed;

        public FileProviderClientCache()
            : this(DefaultIdleTimeout, DefaultGracePeriod)
        {
        }

        public FileProviderClientCache(TimeSpan idleTimeout, TimeSpan gracePeriod)
        {
            if (idleTimeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(idleTimeout), "Idle timeout must be greater than zero.");
            }

            if (gracePeriod < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(gracePeriod), "Grace period must be greater than or equal to zero.");
            }

            _idleTimeout = idleTimeout;
            _gracePeriod = gracePeriod;

            var sweepInterval = TimeSpan.FromTicks(idleTimeout.Ticks / 2);
            if (sweepInterval < TimeSpan.FromMilliseconds(10))
            {
                sweepInterval = TimeSpan.FromMilliseconds(10);
            }
            _sweepInterval = sweepInterval > MaxSweepInterval ? MaxSweepInterval : sweepInterval;
            _nextSweepTicks = DateTime.UtcNow.Ticks;
        }

        public virtual TClient GetOrAdd(string key, Func<TClient> factory)
        {
            Check.NotNullOrWhiteSpace(key, nameof(key));
            Check.NotNull(factory, nameof(factory));
            ThrowIfDisposed();

            SweepIfNeeded();

            var lazy = _syncEntries.GetOrAdd(
                key,
                _ => new Lazy<Entry>(() => new Entry(factory()), LazyThreadSafetyMode.ExecutionAndPublication));

            Entry entry;
            try
            {
                entry = lazy.Value;
            }
            catch
            {
                ((ICollection<KeyValuePair<string, Lazy<Entry>>>)_syncEntries).Remove(new(key, lazy));
                throw;
            }

            if (Volatile.Read(ref _disposed) == 1)
            {
                ((ICollection<KeyValuePair<string, Lazy<Entry>>>)_syncEntries).Remove(new(key, lazy));
                DisposeEntry(entry);
                ThrowIfDisposed();
            }

            entry.Touch();
            return entry.Client;
        }

        public virtual async Task<TClient> GetOrAddAsync(string key, Func<Task<TClient>> factory)
        {
            Check.NotNullOrWhiteSpace(key, nameof(key));
            Check.NotNull(factory, nameof(factory));
            ThrowIfDisposed();

            SweepIfNeeded();

            var lazy = _asyncEntries.GetOrAdd(
                key,
                _ => new Lazy<Task<Entry>>(
                    async () => new Entry(await factory()),
                    LazyThreadSafetyMode.ExecutionAndPublication));

            Entry entry;
            try
            {
                entry = await lazy.Value;
            }
            catch
            {
                ((ICollection<KeyValuePair<string, Lazy<Task<Entry>>>>)_asyncEntries).Remove(new(key, lazy));
                throw;
            }

            if (Volatile.Read(ref _disposed) == 1)
            {
                ((ICollection<KeyValuePair<string, Lazy<Task<Entry>>>>)_asyncEntries).Remove(new(key, lazy));
                DisposeEntry(entry);
                ThrowIfDisposed();
            }

            entry.Touch();
            return entry.Client;
        }

        public virtual void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
            {
                return;
            }

            var errors = new List<Exception>();
            foreach (var pair in _syncEntries)
            {
                if (_syncEntries.TryRemove(pair.Key, out var lazy) && lazy.IsValueCreated)
                {
                    CollectDisposeError(() => DisposeEntry(lazy.Value), errors);
                }
            }

            foreach (var pair in _asyncEntries)
            {
                if (_asyncEntries.TryRemove(pair.Key, out var lazy) &&
                    lazy.IsValueCreated &&
                    lazy.Value.Status == TaskStatus.RanToCompletion)
                {
                    CollectDisposeError(() => DisposeEntry(lazy.Value.Result), errors);
                }
            }

            while (_retiredClients.TryDequeue(out var retired))
            {
                CollectDisposeError(() => DisposeClient(retired.Client), errors);
            }

            if (errors.Count > 0) { throw new AggregateException(errors); }
        }

        protected virtual void SweepIfNeeded()
        {
            var now = DateTime.UtcNow.Ticks;
            var next = Interlocked.Read(ref _nextSweepTicks);

            if (now < next)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _nextSweepTicks, now + _sweepInterval.Ticks, next) != next)
            {
                return;
            }

            Sweep(now);
        }

        protected virtual void Sweep(long now)
        {
            DisposeDueRetiredClients(now);

            foreach (var pair in _syncEntries)
            {
                if (pair.Value.IsValueCreated && now - pair.Value.Value.LastUsedTicks >= _idleTimeout.Ticks)
                {
                    if (((ICollection<KeyValuePair<string, Lazy<Entry>>>)_syncEntries).Remove(pair) &&
                        pair.Value.Value.TryRelease())
                    {
                        Retire(pair.Value.Value.Client, now);
                    }
                }
            }

            foreach (var pair in _asyncEntries)
            {
                if (!pair.Value.IsValueCreated || pair.Value.Value.Status != TaskStatus.RanToCompletion)
                {
                    continue;
                }

                var entry = pair.Value.Value.Result;
                if (now - entry.LastUsedTicks >= _idleTimeout.Ticks)
                {
                    if (((ICollection<KeyValuePair<string, Lazy<Task<Entry>>>>)_asyncEntries).Remove(pair) &&
                        entry.TryRelease())
                    {
                        Retire(entry.Client, now);
                    }
                }
            }

            // With a zero grace period the retired clients from this sweep are due immediately.
            DisposeDueRetiredClients(now);
        }

        protected virtual void Retire(TClient client, long now)
        {
            _retiredClients.Enqueue(new RetiredClient(client, now + _gracePeriod.Ticks));
            if (Volatile.Read(ref _disposed) == 1)
            {
                DisposeDueRetiredClients(long.MaxValue);
            }
        }

        protected virtual void DisposeDueRetiredClients(long now)
        {
            var errors = new List<Exception>();
            while (_retiredClients.TryPeek(out var retired) && retired.DisposeAtTicks <= now)
            {
                if (_retiredClients.TryDequeue(out retired))
                {
                    CollectDisposeError(() => DisposeClient(retired.Client), errors);
                }
            }

            if (errors.Count > 0) { throw new AggregateException(errors); }
        }

        private static void CollectDisposeError(Action cleanup, List<Exception> errors)
        {
            try { cleanup(); }
            catch (Exception exception) { errors.Add(exception); }
        }

        protected virtual void DisposeClient(TClient client)
        {
            if (client is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void DisposeEntry(Entry entry)
        {
            if (entry.TryRelease())
            {
                DisposeClient(entry.Client);
            }
        }

        protected virtual void ThrowIfDisposed()
        {
            if (Volatile.Read(ref _disposed) == 1)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
