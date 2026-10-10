using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IOperationRateLimitingChecker
    {
        Task CheckAsync(string policyName, OperationRateLimitingContext? context = null,
            CancellationToken cancellationToken = default);
        Task<bool> IsAllowedAsync(string policyName, OperationRateLimitingContext? context = null,
            CancellationToken cancellationToken = default);
        Task<OperationRateLimitingStatus> GetStatusAsync(string policyName, OperationRateLimitingContext? context = null,
            CancellationToken cancellationToken = default);
        Task ResetAsync(string policyName, OperationRateLimitingContext? context = null,
            CancellationToken cancellationToken = default);
    }
}
