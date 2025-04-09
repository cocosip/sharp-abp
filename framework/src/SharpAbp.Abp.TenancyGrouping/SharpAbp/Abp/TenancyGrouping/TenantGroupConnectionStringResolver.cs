using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.TenancyGrouping
{
    [Dependency(ReplaceServices = true)]
    public class TenantGroupConnectionStringResolver : MultiTenantConnectionStringResolver
    {
        protected IServiceProvider ServiceProvider { get; }
        public TenantGroupConnectionStringResolver(
            IOptionsMonitor<AbpDbConnectionOptions> options,
            ICurrentTenant currentTenant,
            IServiceProvider serviceProvider) : base(options, currentTenant, serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }


        protected override async Task<TenantConfiguration?> FindTenantConfigurationAsync(Guid tenantId)
        {
            using var serviceScope = ServiceProvider.CreateScope();

            var tenantStore = serviceScope
                .ServiceProvider
                .GetRequiredService<ITenantStore>();

            var tenant = await tenantStore.FindAsync(tenantId);

            var tenantGroupStore = serviceScope.ServiceProvider.GetRequiredService<ITenantGroupStore>();
            var tenantGroup = await tenantGroupStore.FindByTenantIdAsync(tenantId);
            if (tenantGroup != null && tenantGroup.IsActive)
            {
                if (tenantGroup.ConnectionStrings != null)
                {
                    var defaultConnectionString = tenant?.ConnectionStrings?.Default;

                    if (tenant != null)
                    {
                        tenant.ConnectionStrings = tenantGroup.ConnectionStrings;
                        if (tenant.ConnectionStrings.Default.IsNullOrWhiteSpace())
                        {
                            tenant.ConnectionStrings.Default = defaultConnectionString;
                        }
                    }
                }
            }

            return tenant;
        }

        [Obsolete("Use FindTenantConfigurationAsync method.")]
        protected override TenantConfiguration? FindTenantConfiguration(Guid tenantId)
        {
            using var serviceScope = ServiceProvider.CreateScope();
            var tenantStore = serviceScope
                .ServiceProvider
                .GetRequiredService<ITenantStore>();

            var tenant = tenantStore.Find(tenantId);

            var tenantGroupStore = serviceScope.ServiceProvider.GetRequiredService<ITenantGroupStore>();
            var tenantGroup = tenantGroupStore.FindByTenantId(tenantId);
            if (tenantGroup != null && tenantGroup.IsActive)
            {
                if (tenant != null)
                {
                    tenant.ConnectionStrings = tenantGroup.ConnectionStrings;
                }
            }

            return tenant;
        }

    }
}
