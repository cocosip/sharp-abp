using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [Route("api/identity/users")]
    public class IdentityUserController : IdentityController, IIdentityUserAppService
    {
        private readonly IIdentityUserAppService _identityUserAppService;
        public IdentityUserController(IIdentityUserAppService identityUserAppService)
        {
            _identityUserAppService = identityUserAppService;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IdentityUserDto> GetAsync(Guid id)
        {
            return await _identityUserAppService.GetAsync(id);
        }

        /// <summary>
        /// Find by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find-by-email/{email}")]
        public async Task<IdentityUserDto> FindByEmailAsync(string email)
        {
            return await _identityUserAppService.FindByEmailAsync(email);
        }

        /// <summary>
        /// 根据id列表获取用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-list-by-ids")]
        public async Task<List<IdentityUserDto>> GetListByIdsAsync(List<Guid> ids)
        {
            return await _identityUserAppService.GetListByIdsAsync(ids);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
        {
            return await _identityUserAppService.GetListAsync(input);
        }

        /// <summary>
        /// Find by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find-by-username/{userName}")]
        public async Task<IdentityUserDto> FindByUsernameAsync(string userName)
        {
            return await _identityUserAppService.FindByUsernameAsync(userName);
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IdentityUserDto> CreateAsync(NewIdentityUserCreateDto input)
        {
            return await _identityUserAppService.CreateAsync(input);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IdentityUserDto> UpdateAsync(Guid id, NewIdentityUserUpdateDto input)
        {
            return await _identityUserAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _identityUserAppService.DeleteAsync(id);
        }

        /// <summary>
        /// Get all claim types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all-claim-types")]
        public async Task<List<IdentityClaimTypeDto>> GetAllClaimTypesAsync()
        {
            return await _identityUserAppService.GetAllClaimTypesAsync();
        }

        /// <summary>
        /// Get roles
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/roles")]
        public async Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id)
        {
            return await _identityUserAppService.GetRolesAsync(id);
        }

        /// <summary>
        /// assignable-roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("assignable-roles")]
        public async Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
        {
            return await _identityUserAppService.GetAssignableRolesAsync();
        }


        /// <summary>
        /// Update roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/roles")]
        public async Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input)
        {
            await _identityUserAppService.UpdateRolesAsync(id, input);
        }

        /// <summary>
        /// Get claims
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/claims")]
        public async Task<List<IdentityUserClaimDto>> GetClaimsAsync(Guid id)
        {
            return await _identityUserAppService.GetClaimsAsync(id);
        }

        /// <summary>
        /// Update claims
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityUserClaims"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/claims")]
        public async Task UpdateClaimsAsync(Guid id, List<CreateOrUpdateIdentityUserClaimDto> identityUserClaims)
        {
            await _identityUserAppService.UpdateClaimsAsync(id, identityUserClaims);
        }


        /// <summary>
        /// Get organization-units
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/organization-units")]
        public async Task<List<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id)
        {
            return await _identityUserAppService.GetOrganizationUnitsAsync(id);
        }

        /// <summary>
        /// Get available-organization-units
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("available-organization-units")]
        public async Task<ListResultDto<OrganizationUnitDto>> GetAvailableOrganizationUnitsAsync()
        {
            return await _identityUserAppService.GetAvailableOrganizationUnitsAsync();
        }

        /// <summary>
        /// Lock user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/lock/{seconds}")]
        public async Task LockAsync(Guid id, int seconds)
        {
            await _identityUserAppService.LockAsync(id, seconds);
        }

        /// <summary>
        /// Unlock
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/unlock")]
        public async Task UnLockAsync(Guid id)
        {
            await _identityUserAppService.UnLockAsync(id);
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/change-password")]
        public async Task SetPasswordAsync(Guid id, SetPasswordDto input)
        {
            await _identityUserAppService.SetPasswordAsync(id, input);
        }

        /// <summary>
        /// two-factor-enabled
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/two-factor-enabled")]
        public async Task<bool> TwoFactorEnableAsync(Guid id)
        {
            return await _identityUserAppService.TwoFactorEnableAsync(id);
        }

        /// <summary>
        /// set two-factor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/two-factor/{enabled}")]
        public async Task SetTwoFactorAsync(Guid id, bool enabled)
        {
            await _identityUserAppService.SetTwoFactorAsync(id, enabled);
        }

    }
}
