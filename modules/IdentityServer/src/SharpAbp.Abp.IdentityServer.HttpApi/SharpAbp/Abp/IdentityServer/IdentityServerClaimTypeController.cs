using Microsoft.AspNetCore.Mvc;
using SharpAbp.Abp.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.IdentityServer
{
    [RemoteService(Name = IdentityServerRemoteServiceConsts.RemoteServiceName)]
    [Area("identity-server")]
    [Route("api/identity-server/claim-types")]
    public class IdentityServerClaimTypeController : IdentityServerController, IIdentityServerClaimTypeAppService
    {
        private readonly IIdentityServerClaimTypeAppService _identityServerClaimTypeAppService;
        public IdentityServerClaimTypeController(IIdentityServerClaimTypeAppService identityServerClaimTypeAppService)
        {
            _identityServerClaimTypeAppService = identityServerClaimTypeAppService;
        }

        /// <summary>
        /// Get claim-types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<IdentityClaimTypeDto>> GetAllAsync()
        {
            return await _identityServerClaimTypeAppService.GetAllAsync();
        }
    }
}
