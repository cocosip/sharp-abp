using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [Route("api/identity/roles")]
    public class IdentityRoleController : IdentityController, IIdentityRoleAppService
    {
        private readonly IIdentityRoleAppService _identityRoleAppService;
        public IdentityRoleController(IIdentityRoleAppService identityRoleAppService)
        {
            _identityRoleAppService = identityRoleAppService;
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
