using SharpAbp.Abp.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.IdentityServer
{
    public interface IIdentityServerClaimTypeAppService : IApplicationService
    {

        /// <summary>
        /// Get all claim-types
        /// </summary>
        /// <returns></returns>
        Task<List<IdentityClaimTypeDto>> GetAllAsync();
    }
}
