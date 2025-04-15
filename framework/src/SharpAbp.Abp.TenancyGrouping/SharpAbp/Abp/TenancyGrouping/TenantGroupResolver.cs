using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class TenantGroupResolver : ITenantGroupResolver, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AbpTenantGroupResolveOptions _options;

        public TenantGroupResolver(IOptions<AbpTenantGroupResolveOptions> options, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        public virtual async Task<TenantGroupResolveResult> ResolveGroupIdOrNameAsync()
        {
            var result = new TenantGroupResolveResult();

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var context = new TenantGroupResolveContext(serviceScope.ServiceProvider);

                foreach (var tenantResolver in _options.TenantResolvers)
                {
                    await tenantResolver.ResolveAsync(context);

                    result.AppliedResolvers.Add(tenantResolver.Name);

                    if (context.HasResolvedTenantOrHost())
                    {
                        result.GroupIdOrName = context.GroupIdOrName;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
