using System;
using System.Threading.RateLimiting;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    /// <summary>Owns shared native partition limiters for one application container.</summary>
    public interface IConcurrencyLimiterRegistry : IDisposable
    {
        PartitionedRateLimiter<ConcurrencyLimitingPartitionKey> Get(ConcurrencyLimitingPolicy policy);
    }

}
