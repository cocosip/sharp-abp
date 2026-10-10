using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IOperationRateLimitingStore
    {
        Task<OperationRateLimitingStoreResult> IncrementAsync(string key, TimeSpan duration, int maxCount,
            CancellationToken cancellationToken = default);
        Task<OperationRateLimitingStoreState> GetAsync(string key, CancellationToken cancellationToken = default);
        Task ResetAsync(string key, CancellationToken cancellationToken = default);
    }
}
