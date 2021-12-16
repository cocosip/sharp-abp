using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.Account
{
    [RemoteService(Name = AccountRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [ControllerName("Profile")]
    [Route("api/identity/my-profile")]
    public class ProfileController : AbpController, IProfileAppService
    {
        private readonly IProfileAppService _profileAppService;

        public ProfileController(IProfileAppService profileAppService)
        {
            _profileAppService = profileAppService;
        }

        [HttpGet]
        public virtual Task<ProfileDto> GetAsync()
        {
            return _profileAppService.GetAsync();
        }

        [HttpPut]
        public virtual Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
        {
            return _profileAppService.UpdateAsync(input);
        }

        [HttpPost]
        [Route("change-password")]
        public virtual Task ChangePasswordAsync(ChangePasswordInput input)
        {
            return _profileAppService.ChangePasswordAsync(input);
        }
    }
}