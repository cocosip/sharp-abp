using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantManager : DomainService, IMapTenantManager
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
        /// Create MapTenant
        /// </summary>
        /// <param name="mapTenant"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(MapTenant mapTenant)
        {
            //Validate
            await ValidateTenantAsync(mapTenant.TenantId);
            await ValidateCodeAsync(mapTenant.Code);
            await ValidateMapCodeAsync(mapTenant.MapCode);

            await MapTenantRepository.InsertAsync(mapTenant);
        }

        /// <summary>
        /// Update MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="tenantId"></param>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(Guid id, string code, Guid tenantId, string mapCode)
        {
            var mapTenant = await MapTenantRepository.FindAsync(id);
            if (mapTenant == null)
            {
                throw new AbpException($"Could not find 'MapTenant' by id '{id}'.");
            }
            //Validate
            await ValidateTenantAsync(tenantId, id);
            await ValidateCodeAsync(code, id);
            await ValidateMapCodeAsync(mapCode, id);

            mapTenant.Update(code, tenantId, mapCode);

            await MapTenantRepository.UpdateAsync(mapTenant);
        }


        /// <summary>
        /// Validate tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        protected virtual async Task ValidateTenantAsync(Guid tenantId, Guid? expectedId = null)
        {
            var tenant = await TenantRepository.FindAsync(tenantId, false);
            if (tenant == null)
            {
                throw new AbpException($"Can't find any tenant by '{tenantId}'.");
            }

            var mapTenant = await MapTenantRepository.FindExpectedTenantIdAsync(tenantId, expectedId);
            if (mapTenant != null)
            {
                throw new AbpException($"Duplicate 'MapTenant' tenantId: '{tenantId}'.");
            }
        }

        /// <summary>
        /// Validate code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        protected virtual async Task ValidateCodeAsync(string code, Guid? expectedId = null)
        {
            var mapTenant = await MapTenantRepository.FindExpectedCodeAsync(code, expectedId);
            if (mapTenant != null)
            {
                throw new AbpException($"Duplicate 'MapTenant' code:'{code}'.");
            }
        }

        protected virtual async Task ValidateMapCodeAsync(string mapCode, Guid? expectedId = null)
        {
            var mapTenant = await MapTenantRepository.FindExpectedCodeAsync(mapCode, expectedId);
            if (mapTenant != null)
            {
                throw new AbpException($"Duplicate 'MapTenant' mapCode:'{mapCode}'.");
            }
        }

    }
}
