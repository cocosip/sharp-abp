using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [Route("api/identity/settings")]
    public class IdentitySettingsController : IdentityController, IIdentitySettingsAppService
    {
        private readonly IIdentitySettingsAppService _identitySettingsAppService;
        public IdentitySettingsController(IIdentitySettingsAppService identitySettingsAppService)
        {
            _identitySettingsAppService = identitySettingsAppService;
        }

        [HttpGet]
        public async Task<IdentitySettingsDto> GetAsync()
        {
            return await _identitySettingsAppService.GetAsync();
        }

        [HttpPut]
        public async Task UpdateAsync(UpdateIdentitySettingsDto input)
        {
            await _identitySettingsAppService.UpdateAsync(input);
        }
    }
}
