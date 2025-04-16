using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class DefaultTenantGroupResolveContributor : TenantGroupResolveContributorBase
    {
        public override string Name => "Default";

        public override async Task ResolveAsync(ITenantGroupResolveContext context)
        {
            try
            {
                var currentTenant = context.ServiceProvider.GetRequiredService<ICurrentTenant>();
                if (currentTenant?.Id != null)
                {
                    var tenantGroupStore = context.ServiceProvider.GetRequiredService<ITenantGroupStore>();
                    var tenantGroupConfiguration = await tenantGroupStore.FindByTenantIdAsync(currentTenant.GetId());
                    context.GroupIdOrName = tenantGroupConfiguration?.Id.ToString() ?? tenantGroupConfiguration?.NormalizedName;
                    context.Handled = context.GroupIdOrName != null;
                }
            }
            catch (Exception e)
            {
                context.ServiceProvider
                    .GetRequiredService<ILogger<DefaultTenantGroupResolveContributor>>()
                    .LogWarning(e.ToString());
            }

        }
    }
}
