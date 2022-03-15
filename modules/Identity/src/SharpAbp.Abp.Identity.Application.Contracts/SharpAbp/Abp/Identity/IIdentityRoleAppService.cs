using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    public interface IIdentityRoleAppService : ICrudAppService<
        IdentityRoleDto,
        Guid,
        GetIdentityRolesInput,
        IdentityRoleCreateDto,
        IdentityRoleUpdateDto>
    {


        /// <summary>
        /// Get all claimTypes
        /// </summary>
        /// <returns></returns>
        Task<List<IdentityClaimTypeDto>> GetAllClaimTypesAsync();

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


        Task<ListResultDto<IdentityRoleDto>> GetAllListAsync();
    }
}
