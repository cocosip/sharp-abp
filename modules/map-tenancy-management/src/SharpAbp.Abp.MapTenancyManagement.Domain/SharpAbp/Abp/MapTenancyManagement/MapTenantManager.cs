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
        /// Validate tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        public virtual async Task ValidateTenantAsync(Guid tenantId, Guid? expectedId = null)
        {
            var tenant = await TenantRepository.FindAsync(tenantId, false);
            if (tenant == null)
            {
                throw new UserFriendlyException($"Can't find any tenant by '{tenantId}'.");
            }

            var mapTenant = await MapTenantRepository.FindExpectedTenantIdAsync(tenantId, expectedId);
            if (mapTenant != null)
            {
                throw new UserFriendlyException($"Duplicate tenantId: '{tenantId}'.");
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
                throw new UserFriendlyException($"The 'MapTenant' code was exist! Code:{code}.");
            }
        }

    }
}
