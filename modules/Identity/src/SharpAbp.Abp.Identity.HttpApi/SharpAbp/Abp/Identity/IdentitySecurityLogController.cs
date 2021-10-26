using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [Route("api/identity/security-logs")]
    public class IdentitySecurityLogController : IdentityController, IIdentitySecurityLogAppService
    {
        private readonly IIdentitySecurityLogAppService _identitySecurityLogAppService;
        public IdentitySecurityLogController(IIdentitySecurityLogAppService identitySecurityLogAppService)
        {
            _identitySecurityLogAppService = identitySecurityLogAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IdentitySecurityLogDto> GetAsync(Guid id)
        {
            return await _identitySecurityLogAppService.GetAsync(id);
        }

        [HttpGet]
        public async Task<PagedResultDto<IdentitySecurityLogDto>> GetPagedListAsync([FromQuery] IdentitySecurityLogPagedRequestDto input)
        {
            return await _identitySecurityLogAppService.GetPagedListAsync(input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _identitySecurityLogAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("my")]
        public async Task<PagedResultDto<IdentitySecurityLogDto>> GetMyListAsync(IdentitySecurityLogPagedRequestDto input)
        {
            input.UserId = CurrentTenant.Id;
            return await _identitySecurityLogAppService.GetPagedListAsync(input);
        }

    }
}
