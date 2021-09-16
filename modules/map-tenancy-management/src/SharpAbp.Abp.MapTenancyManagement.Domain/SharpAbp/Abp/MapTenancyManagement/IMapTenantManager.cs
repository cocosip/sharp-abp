using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenantManager : IDomainService
    {
        /// <summary>
        /// Create MapTenant
        /// </summary>
        /// <param name="mapTenant"></param>
        /// <returns></returns>
        Task CreateAsync(MapTenant mapTenant);

        /// <summary>
        /// Update MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="tenantId"></param>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, string code, Guid tenantId, string mapCode);

        /// <summary>
        /// Validate tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        Task ValidateTenantAsync(Guid tenantId, Guid? expectedId = null);

        /// <summary>
        /// Validate code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        Task ValidateCodeAsync(string code, Guid? expectedId = null);

        /// <summary>
        /// Validate mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="expectedId"></param>
        /// <returns></returns>
        Task ValidateMapCodeAsync(string mapCode, Guid? expectedId = null);
    }
}
