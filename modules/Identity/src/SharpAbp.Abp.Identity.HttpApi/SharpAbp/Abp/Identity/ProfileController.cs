//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using Volo.Abp;
//using Volo.Abp.Identity;

//namespace SharpAbp.Abp.Identity
//{
//    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
//    [Area("identity")]
//    [ControllerName("Profile")]
//    [Route("api/identity/my-profile")]
//    public class ProfileController : IdentityController, IProfileAppService
//    {
//        protected IProfileAppService ProfileAppService { get; }

//        public ProfileController(IProfileAppService profileAppService)
//        {
//            ProfileAppService = profileAppService;
//        }

//        [HttpGet]
//        public virtual Task<ProfileDto> GetAsync()
//        {
//            return ProfileAppService.GetAsync();
//        }

//        [HttpPut]
//        public virtual Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
//        {
//            return ProfileAppService.UpdateAsync(input);
//        }

//        [HttpPost]
//        [Route("change-password")]
//        public virtual Task ChangePasswordAsync(ChangePasswordInput input)
//        {
//            return ProfileAppService.ChangePasswordAsync(input);
//        }
//    }
//}
