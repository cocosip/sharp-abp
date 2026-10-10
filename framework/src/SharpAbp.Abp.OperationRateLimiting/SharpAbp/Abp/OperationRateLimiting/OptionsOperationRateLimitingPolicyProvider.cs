using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OptionsOperationRateLimitingPolicyProvider : IOperationRateLimitingPolicyProvider, ISingletonDependency
    {
        protected OperationRateLimitingConfiguration Configuration { get; }

        public OptionsOperationRateLimitingPolicyProvider(OperationRateLimitingConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual Task<OperationRateLimitingPolicy> GetAsync(string policyName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!Configuration.Policies.TryGetValue(policyName, out var policy))
            {
                throw new OperationRateLimitingConfigurationException("Unknown policy: " + policyName);
            }
            return Task.FromResult(policy);
        }
    }
}
