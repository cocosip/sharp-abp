using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Integration;
using Volo.Abp.Users;

namespace SharpAbp.Abp.Identity
{
    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [Route("api/identity/users/integration")]
    public class IdentityUserIntegrationController : IdentityController, IIdentityUserIntegrationService
    {
        protected IIdentityUserIntegrationService IdentityUserIntegrationService { get; }

        public IdentityUserIntegrationController(IIdentityUserIntegrationService identityUserIntegrationService)
        {
            IdentityUserIntegrationService = identityUserIntegrationService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<UserData> FindByIdAsync(Guid id)
        {
            return await IdentityUserIntegrationService.FindByIdAsync(id);
        }

        [HttpGet]
        [Route("by-username/{userName}")]
        public async Task<UserData> FindByUserNameAsync(string userName)
        {
            return await IdentityUserIntegrationService.FindByUserNameAsync(userName);
        }

        [HttpGet]
        [Route("search")]
        public async Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input)
        {
            return await IdentityUserIntegrationService.SearchAsync(input);
        }

        [HttpGet]
        [Route("count")]
        public async Task<long> GetCountAsync(UserLookupCountInputDto input)
        {
            return await IdentityUserIntegrationService.GetCountAsync(input);
        }

        [HttpGet]
        [Route("role-names")]
        public async Task<string[]> GetRoleNamesAsync(Guid id)
        {
            return await IdentityUserIntegrationService.GetRoleNamesAsync(id);
        }
    }
}
