using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.Identity
{
    public interface IIdentityRoleAppService : IApplicationService
    {

        /// <summary>
        /// Get role claims
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<IdentityRoleClaimDto>> GetClaimsAsync(Guid id);

        /// <summary>
        /// Create or Update claims
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleClaims"></param>
        /// <returns></returns>
        Task CreateOrUpdateClaimsAsync(Guid id, List<CreateOrUpdateIdentityRoleClaimDto> roleClaims);
    
    }
}
