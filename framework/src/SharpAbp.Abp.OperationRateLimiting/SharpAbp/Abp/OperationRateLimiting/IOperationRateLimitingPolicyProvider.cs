using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IOperationRateLimitingPolicyProvider
    {
        Task<OperationRateLimitingPolicy> GetAsync(string policyName, CancellationToken cancellationToken = default);
    }
}
