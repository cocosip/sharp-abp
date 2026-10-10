using System;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingPartitionResolver : IConcurrencyLimitingPartitionResolver, ITransientDependency
    {
        protected IServiceProvider Services { get; }
        protected ICurrentTenant CurrentTenant { get; }
        protected ICurrentUser CurrentUser { get; }
        protected IConcurrencyLimitingConfiguration Configuration { get; }

        public ConcurrencyLimitingPartitionResolver(
            IServiceProvider services,
            ICurrentTenant tenant,
            ICurrentUser user,
            IConcurrencyLimitingConfiguration configuration)
        {
            Services = services;
            CurrentTenant = tenant;
            CurrentUser = user;
            Configuration = configuration;
        }

        public virtual ConcurrencyLimitingPartitionKey Resolve(ConcurrencyLimitingPolicy policy, ConcurrencyLimitingContext context)
        {
            string? key;
            switch (policy.PartitionKind)
            {
                case ConcurrencyLimitingPartitionKind.Global:
                    key = "global";
                    break;
                case ConcurrencyLimitingPartitionKind.Parameter:
                    key = context.Parameter;
                    break;
                case ConcurrencyLimitingPartitionKind.CurrentTenant:
                    key = CurrentTenant.Id?.ToString("D") ?? "host";
                    break;
                case ConcurrencyLimitingPartitionKind.CurrentUser:
                    key = CurrentUser.Id?.ToString("D");
                    break;
                case ConcurrencyLimitingPartitionKind.Custom:
                    key = Configuration.Resolve(policy.ResolverName!, Services, context);
                    break;
                default:
                    throw new ConcurrencyLimitingConfigurationException("Unknown concurrency partition kind.");
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ConcurrencyLimitingContextException("Concurrency partition requires a nonempty key: " + policy.Name);
            }
            return new ConcurrencyLimitingPartitionKey(key!, policy.IsMultiTenant ? CurrentTenant.Id : null);
        }
    }

}
