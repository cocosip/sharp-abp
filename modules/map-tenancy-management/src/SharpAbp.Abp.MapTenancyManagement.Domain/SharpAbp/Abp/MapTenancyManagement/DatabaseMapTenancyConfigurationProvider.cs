using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancy;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancyManagement
{

    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IMapTenancyConfigurationProvider))]
    public class DatabaseMapTenancyConfigurationProvider : IMapTenancyConfigurationProvider
    {
        protected IMapTenantStore MapTenantStore { get; }
        public DatabaseMapTenancyConfigurationProvider(IMapTenantStore mapTenantStore)
        {
            MapTenantStore = mapTenantStore;
        }

        public virtual async Task<MapTenancyConfiguration> GetAsync([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            var tenant = await MapTenantStore.GetByCodeAsync(code);
            if (tenant != null)
            {
                return new MapTenancyConfiguration(
                    tenant.TenantId,
                    tenant.TenantName,
                    tenant.Code,
                    tenant.MapCode);
            }
            throw new AbpException($"Could not find MapTenancyConfiguration by code '{code}'.");
        }

        public virtual async Task<MapTenancyConfiguration> GetByMapCodeAsync([NotNull] string mapCode)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            var tenant = await MapTenantStore.GetByMapCodeAsync(mapCode);
            if (tenant != null)
            {
                return new MapTenancyConfiguration(
                    tenant.TenantId,
                    tenant.TenantName,
                    tenant.Code,
                    tenant.MapCode);
            }

            throw new AbpException($"Could not find MapTenancyConfiguration by mapCode '{mapCode}'.");
        }
    }
}
