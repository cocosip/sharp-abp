using Microsoft.Extensions.Localization;
using SharpAbp.Abp.MapTenancyManagement.Localization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantManager : DomainService
    {
        protected IStringLocalizer<MapTenancyManagementResource> Localizer { get; }
        protected ITenantRepository TenantRepository { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        public MapTenantManager(
            IStringLocalizer<MapTenancyManagementResource> localizer,
            ITenantRepository tenantRepository,
            IMapTenantRepository mapTenantRepository)
        {
            Localizer = localizer;
            TenantRepository = tenantRepository;
            MapTenantRepository = mapTenantRepository;
        }

        /// <summary>
        /// Create MapTenant
        /// </summary>
        /// <param name="mapTenant"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> CreateAsync(MapTenant mapTenant)
        {
            //Validate
            await ValidateTenantAsync(mapTenant.TenantId);
            await ValidateCodeAsync(mapTenant.Code);
            await ValidateMapCodeAsync(mapTenant.MapCode);

            return await MapTenantRepository.InsertAsync(mapTenant);
        }

        /// <summary>
        /// Update MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="tenantId"></param>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> UpdateAsync(Guid id, string code, Guid tenantId, string mapCode)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);

            //Validate
            await ValidateTenantAsync(tenantId, id);
            await ValidateCodeAsync(code, id);
            await ValidateMapCodeAsync(mapCode, id);

            mapTenant.Update(code, tenantId, mapCode);

            return await MapTenantRepository.UpdateAsync(mapTenant);
        }


        /// <summary>
        /// Validate tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        public virtual async Task ValidateTenantAsync(Guid tenantId, Guid? expectedId = null)
        {
            var tenant = await TenantRepository.GetAsync(tenantId, false);
            var mapTenant = await MapTenantRepository.FindExpectedTenantIdAsync(tenantId, expectedId);
            if (mapTenant != null)
            {
                throw new UserFriendlyException(Localizer["MapTenancyManagement.DuplicateTenantId", tenantId]);
            }
        }

        /// <summary>
        /// Validate code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        public virtual async Task ValidateCodeAsync(string code, Guid? expectedId = null)
        {
            var mapTenant = await MapTenantRepository.FindExpectedCodeAsync(code, expectedId);
            if (mapTenant != null)
            {
                throw new UserFriendlyException(Localizer["MapTenancyManagement.DuplicateCode", code]);
            }
        }

        /// <summary>
        /// Validate mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        public virtual async Task ValidateMapCodeAsync(string mapCode, Guid? expectedId = null)
        {
            var mapTenant = await MapTenantRepository.FindExpectedCodeAsync(mapCode, expectedId);
            if (mapTenant != null)
            {
                throw new UserFriendlyException(Localizer["MapTenancyManagement.DuplicateMapCode", mapCode]);
            }
        }

    }
}
