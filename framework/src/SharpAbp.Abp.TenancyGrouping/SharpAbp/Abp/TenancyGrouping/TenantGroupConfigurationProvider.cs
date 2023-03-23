using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class TenantGroupConfigurationProvider : ITenantGroupConfigurationProvider, ITransientDependency
    {
        protected virtual ITenantGroupStore TenantGroupStore { get; }
        public TenantGroupConfigurationProvider(ITenantGroupStore tenantGroupStore)
        {
            TenantGroupStore = tenantGroupStore;
        }

        public virtual async Task<TenantGroupConfiguration> GetAsync([NotNull] string tenantGroupIdOrName)
        {
            Check.NotNullOrWhiteSpace(tenantGroupIdOrName, nameof(tenantGroupIdOrName));

            var tenantGroup = await FindTenantAsync(tenantGroupIdOrName);
            if (tenantGroup == null)
            {
                throw new BusinessException(
                    code: "Volo.AbpIo.TenancyGrouping:010001",
                    message: "Tenant group not found!",
                    details: "There is no tenant group with the tenant group id or name: " + tenantGroupIdOrName
                );
            }

            if (!tenantGroup.IsActive)
            {
                throw new BusinessException(
                    code: "Volo.AbpIo.TenancyGrouping:010002",
                    message: "Tenant group not active!",
                    details: "The tenant group is no active with the tenant group id or name: " + tenantGroupIdOrName
                );
            }

            return tenantGroup;
        }

        protected virtual async Task<TenantGroupConfiguration> FindTenantAsync(string tenantGroupIdOrName)
        {
            if (Guid.TryParse(tenantGroupIdOrName, out var parsedTenantId))
            {
                return await TenantGroupStore.FindAsync(parsedTenantId);
            }
            else
            {
                return await TenantGroupStore.FindAsync(tenantGroupIdOrName);
            }
        }

    }
}
