using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace SharpAbp.Abp.OperationRateLimiting.Fakes
{
    public class ControllableCache : IDistributedCache
    {
        private readonly ConcurrentDictionary<string, byte[]> _items = new ConcurrentDictionary<string, byte[]>();
        public bool FailRead { get; set; }
        public bool FailWrite { get; set; }
        public bool FailRemove { get; set; }
        public ConcurrentDictionary<string, DateTimeOffset?> Deadlines { get; } = new ConcurrentDictionary<string, DateTimeOffset?>();
        public int Count => _items.Count;

        public byte[]? Get(string key)
        {
            if (FailRead)
            {
                throw new IOException("cache read failure");
            }
            return _items.TryGetValue(key, out var value) ? (byte[])value.Clone() : null;
        }

        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            await Task.Delay(1, token);
            return Get(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            if (FailWrite)
            {
                throw new IOException("cache write failure");
            }
            _items[key] = (byte[])value.Clone();
            Deadlines[key] = options.AbsoluteExpiration;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            Set(key, value, options);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            if (FailRemove)
            {
                throw new IOException("cache remove failure");
            }
            _items.TryRemove(key, out _);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            Remove(key);
            return Task.CompletedTask;
        }

        public void Refresh(string key)
        {
            throw new NotSupportedException("Fixed windows must never refresh.");
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            throw new NotSupportedException("Fixed windows must never refresh.");
        }
    }
}
