using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
namespace SharpAbp.Abp.Identity
{
    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [Route("api/identity/roles")]
    public class IdentityRoleController : IdentityController, IIdentityRoleAppService
    {
        private readonly IIdentityRoleAppService _identityRoleAppService;
        public IdentityRoleController(
            IIdentityRoleAppService identityRoleAppService)
        {
            _identityRoleAppService = identityRoleAppService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
        {
            return await _identityRoleAppService.GetAllListAsync();
        }

        [HttpGet]
        public async Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRolesInput input)
        {
            return await _identityRoleAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IdentityRoleDto> GetAsync(Guid id)
        {
            return await _identityRoleAppService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
        {
            return await _identityRoleAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
        {
            return await _identityRoleAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _identityRoleAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("all-claim-types")]
        public async Task<List<IdentityClaimTypeDto>> GetAllClaimTypesAsync()
        {
            return await _identityRoleAppService.GetAllClaimTypesAsync();
        }

        [HttpGet]
        [Route("{id}/claims")]
        public async Task<List<IdentityRoleClaimDto>> GetClaimsAsync(Guid id)
        {
            return await _identityRoleAppService.GetClaimsAsync(id);
        }

        [HttpPut]
        [Route("{id}/claims")]
        public async Task CreateOrUpdateClaimsAsync(Guid id, List<CreateOrUpdateIdentityRoleClaimDto> roleClaims)
        {
            await _identityRoleAppService.CreateOrUpdateClaimsAsync(id, roleClaims);
        }

    }
}
