using System;
using System.Collections.Generic;
using System.Threading.RateLimiting;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimiterRegistry : IConcurrencyLimiterRegistry, ISingletonDependency
    {
        private readonly object _gate = new object();
        private readonly Dictionary<string, PartitionedRateLimiter<ConcurrencyLimitingPartitionKey>> _limiters = new Dictionary<string, PartitionedRateLimiter<ConcurrencyLimitingPartitionKey>>(StringComparer.Ordinal);
        private bool _disposed;
        public virtual PartitionedRateLimiter<ConcurrencyLimitingPartitionKey> Get(ConcurrencyLimitingPolicy policy)
        {
            lock (_gate)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(ConcurrencyLimiterRegistry));
                }
                if (!_limiters.TryGetValue(policy.Name, out var limiter))
                {
                    // Only the immutable policy enters this singleton factory. Native partition reuse,
                    // idle detection and cleanup retain active and queued partitions automatically.
                    limiter = PartitionedRateLimiter.Create<ConcurrencyLimitingPartitionKey, ConcurrencyLimitingPartitionKey>(
                        key => RateLimitPartition.GetConcurrencyLimiter(key, _ => new ConcurrencyLimiterOptions
                        {
                            PermitLimit = policy.PermitLimit,
                            QueueLimit = policy.QueueLimit,
                            QueueProcessingOrder = policy.QueueProcessingOrder
                        }));
                    _limiters.Add(policy.Name, limiter);
                }
                return limiter;
            }
        }

        public void Dispose()
        {
            List<PartitionedRateLimiter<ConcurrencyLimitingPartitionKey>> limiters;
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;
                limiters = new List<PartitionedRateLimiter<ConcurrencyLimitingPartitionKey>>(_limiters.Values);
                _limiters.Clear();
            }
            foreach (var limiter in limiters)
            {
                limiter.Dispose();
            }
        }
    }

}
