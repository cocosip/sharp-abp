using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [Route("api/identity/claim-types")]
    public class IdentityClaimTypeController : IdentityController, IIdentityClaimTypeAppService
    {
        private readonly IIdentityClaimTypeAppService _identityClaimTypeAppService;
        public IdentityClaimTypeController(IIdentityClaimTypeAppService identityClaimTypeAppService)
        {
            _identityClaimTypeAppService = identityClaimTypeAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IdentityClaimTypeDto> GetAsync(Guid id)
        {
            return await _identityClaimTypeAppService.GetAsync(id);
        }

        [HttpGet]
        public async Task<PagedResultDto<IdentityClaimTypeDto>> GetPagedListAsync(IdentityClaimTypePagedRequestDto input)
        {
            return await _identityClaimTypeAppService.GetPagedListAsync(input);
        }
    }
}
