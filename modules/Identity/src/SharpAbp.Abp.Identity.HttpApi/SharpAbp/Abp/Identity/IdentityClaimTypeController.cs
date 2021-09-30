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
        [Route("any/{name}")]
        public async Task<bool> AnyAsync(string name, Guid? ignoredId = null)
        {
            return await _identityClaimTypeAppService.AnyAsync(name, ignoredId);
        }

        [HttpGet]
        public async Task<PagedResultDto<IdentityClaimTypeDto>> GetPagedListAsync(IdentityClaimTypePagedRequestDto input)
        {
            return await _identityClaimTypeAppService.GetPagedListAsync(input);
        }

        [HttpPost]
        public async Task<Guid> CreateAsync(CreateIdentityClaimTypeDto input)
        {
            return await _identityClaimTypeAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateIdentityClaimTypeDto input)
        {
            await _identityClaimTypeAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _identityClaimTypeAppService.DeleteAsync(id);
        }
    }
}
