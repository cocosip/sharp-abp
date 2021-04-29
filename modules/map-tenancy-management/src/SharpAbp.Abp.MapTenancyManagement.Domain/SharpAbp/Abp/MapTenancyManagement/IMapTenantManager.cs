using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenantManager : IDomainService
    {
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
    }
}
