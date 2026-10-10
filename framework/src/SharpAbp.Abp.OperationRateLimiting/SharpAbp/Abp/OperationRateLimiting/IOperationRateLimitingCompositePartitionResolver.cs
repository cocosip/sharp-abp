using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IOperationRateLimitingCompositePartitionResolver
    {
        Task<IReadOnlyList<string>> ResolvePartitionsAsync(OperationRateLimitingRuleDefinition rule,
            OperationRateLimitingContext context, CancellationToken cancellationToken = default);
    }
}
