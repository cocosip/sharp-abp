using System;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [CacheName("SharpAbp.OperationRateLimiting")]
    [IgnoreMultiTenancy]
    public class OperationRateLimitingCacheItem
    {
        public int CurrentCount { get; set; }
        public DateTime WindowEndUtc { get; set; }
    }
}
