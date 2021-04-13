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
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task ValidateTenantAsync(Guid id)
        {
            var tenant = await TenantRepository.FindAsync(id, false);
            if (tenant == null)
            {
                throw new AbpException($"MapTenant tenant {id} was not exist.");
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
            var mapTenant = await MapTenantRepository.FindExpectedAsync(code, expectedId);
            if (mapTenant != null)
            {
                throw new AbpException($"The 'MapTenant' code was exist! Code:{code}.");
            }
        }


    }
}
