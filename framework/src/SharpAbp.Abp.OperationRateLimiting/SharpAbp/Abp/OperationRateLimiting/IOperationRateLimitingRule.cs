using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IOperationRateLimitingRule
    {
        Task<OperationRateLimitingRuleStatus> GetStatusAsync(OperationRateLimitingRuleContext context,
            CancellationToken cancellationToken = default);
        Task<OperationRateLimitingRuleResult> ConsumeAsync(OperationRateLimitingRuleContext context,
            CancellationToken cancellationToken = default);
        Task ResetAsync(OperationRateLimitingRuleContext context, CancellationToken cancellationToken = default);
    }
}
