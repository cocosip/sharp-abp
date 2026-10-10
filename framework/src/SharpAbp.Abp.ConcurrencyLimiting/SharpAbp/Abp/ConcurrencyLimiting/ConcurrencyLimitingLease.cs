using System;
using System.Threading;
using System.Threading.RateLimiting;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    /// <summary>An execution permit whose disposal is idempotent. Failed leases hold no capacity.</summary>
    public sealed class ConcurrencyLimitingLease : IDisposable
    {
        private RateLimitLease? _lease;
        public bool IsAcquired { get; }
        internal ConcurrencyLimitingLease(RateLimitLease? lease)
        {
            _lease = lease;
            IsAcquired = lease?.IsAcquired ?? true;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _lease, null)?.Dispose();
        }
    }

}
