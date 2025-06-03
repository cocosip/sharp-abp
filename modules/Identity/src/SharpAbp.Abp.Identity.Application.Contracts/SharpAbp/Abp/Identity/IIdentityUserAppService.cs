using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    public interface IIdentityUserAppService : IApplicationService
    {
        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //TODO: [Authorize(IdentityPermissions.Users.Default)] should go the IdentityUserAppService class.
        Task<IdentityUserDto> GetAsync(Guid id);

        /// <summary>
        /// Find by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<IdentityUserDto> FindByUsernameAsync(string userName);

        /// <summary>
        /// Find by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<IdentityUserDto> FindByEmailAsync(string email);

        /// <summary>
        /// Get list by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<IdentityUserDto>> GetListByIdsAsync(List<Guid> ids);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input);

        /// <summary>
        /// Get all claimTypes
        /// </summary>
        /// <returns></returns>
        Task<List<IdentityClaimTypeDto>> GetAllClaimTypesAsync();

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id);

        /// <summary>
        /// Get assignable roles
        /// </summary>
        /// <returns></returns>
        Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync();

        /// <summary>
        /// Get organizationUnits
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id);

        /// <summary>
        /// Get available organizationUnits
        /// </summary>
        /// <returns></returns>
        Task<ListResultDto<OrganizationUnitDto>> GetAvailableOrganizationUnitsAsync();

        /// <summary>
        /// Get claims
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<IdentityUserClaimDto>> GetClaimsAsync(Guid id);

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IdentityUserDto> CreateAsync(NewIdentityUserCreateDto input);

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IdentityUserDto> UpdateAsync(Guid id, NewIdentityUserUpdateDto input);

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Update roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input);

        /// <summary>
        /// Update claims
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityUserClaims"></param>
        /// <returns></returns>
        Task UpdateClaimsAsync(Guid id, List<CreateOrUpdateIdentityUserClaimDto> identityUserClaims);

        /// <summary>
        /// Lock user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        Task LockAsync(Guid id, int seconds);

        /// <summary>
        /// Unlock user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UnLockAsync(Guid id);

        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SetPasswordAsync(Guid id, SetPasswordDto input);

        /// <summary>
        /// two-factor enabled
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> TwoFactorEnableAsync(Guid id);

        /// <summary>
        /// Set two-factor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        Task SetTwoFactorAsync(Guid id, bool enabled);
    }
}
