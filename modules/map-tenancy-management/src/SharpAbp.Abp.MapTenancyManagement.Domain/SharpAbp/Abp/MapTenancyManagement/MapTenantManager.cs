using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantManager : DomainService
    {
        protected ITenantRepository TenantRepository { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        public MapTenantManager(
            ITenantRepository tenantRepository,
            IMapTenantRepository mapTenantRepository)
        {
            TenantRepository = tenantRepository;
            MapTenantRepository = mapTenantRepository;
        }

        /// <summary>
        /// Create mapTenant
        /// </summary>
        /// <param name="mapTenant"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(MapTenant mapTenant)
        {
            //Check tenant exist
            var tenant = await TenantRepository.FindAsync(mapTenant.TenantId, false);
            if (tenant == null)
            {
                throw new AbpException($"Tenant '{mapTenant.TenantId}' is not exist.");
            }
            //Check code
            await ValidateCodeAsync(mapTenant.Code);
            await MapTenantRepository.InsertAsync(mapTenant);
        }

        /// <summary>
        /// Validate code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        public virtual async Task ValidateCodeAsync(string code, Guid? expectedId = null)
        {
            var mapTenant = await MapTenantRepository.FindAsync(code, expectedId);
            if (mapTenant != null)
            {
                throw new AbpException($"The 'MapTenant' was exist! Code:{code}.");
            }
        }

    }
}
