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
    [ControllerName("Role")]
    [Route("api/identity/roles")]
    public class IdentityRoleController : IdentityController, IIdentityRoleAppService, Volo.Abp.Identity.IIdentityRoleAppService
    {
        private readonly IIdentityRoleAppService _identityRoleAppService;
        private readonly Volo.Abp.Identity.IIdentityRoleAppService _roleAppService;
        public IdentityRoleController(
            IIdentityRoleAppService identityRoleAppService,
            Volo.Abp.Identity.IIdentityRoleAppService roleAppService)
        {
            _identityRoleAppService = identityRoleAppService;
            _roleAppService = roleAppService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
        {
            return await _roleAppService.GetAllListAsync();
        }

        [HttpGet]
        public async Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRolesInput input)
        {
            return await _roleAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IdentityRoleDto> GetAsync(Guid id)
        {
            return await _roleAppService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
        {
            return await _roleAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
        {
            return await _roleAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        public async Task DeleteAsync(Guid id)
        {
            await _roleAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("all-claim-types")]
        public async Task<List<IdentityClaimTypeDto>> GetAllClaimTypes()
        {
            return await _identityRoleAppService.GetAllClaimTypes();
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
