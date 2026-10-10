using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IOperationRateLimitingPartitionResolver
    {
        Task<string> ResolveAsync(OperationRateLimitingRuleDefinition rule, OperationRateLimitingContext context,
            CancellationToken cancellationToken = default);
        string GetTenantKey();
    }
}
