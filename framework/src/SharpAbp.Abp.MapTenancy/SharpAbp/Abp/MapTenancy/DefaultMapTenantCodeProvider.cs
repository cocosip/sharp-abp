using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancy
{
    public class DefaultMapTenantCodeProvider : IMapTenantCodeProvider, ITransientDependency
    {
        protected AbpMapTenancyOptions Options { get; }

        public DefaultMapTenantCodeProvider(IOptions<AbpMapTenancyOptions> options)
        {
            Options = options.Value;
        }

        public virtual Task<MapTenantCodeInfo?> FindByTenantIdAsync(Guid tenantId)
        {
            var configuration = Options.Mappers.GetConfigurationByTenantId(tenantId);
            if (configuration?.Code == null)
            {
                return Task.FromResult<MapTenantCodeInfo?>(null);
            }

            return Task.FromResult<MapTenantCodeInfo?>(new MapTenantCodeInfo
            {
                TenantId = tenantId,
                TenantName = configuration.TenantName,
                Code = configuration.Code,
                MapCode = configuration.MapCode
            });
        }
    }
}
