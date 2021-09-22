using Microsoft.AspNetCore.Authorization;
using SharpAbp.Abp.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.IdentityServer
{
    [Authorize]
    public class IdentityServerClaimTypeAppService : IdentityServerAppServiceBase, IIdentityServerClaimTypeAppService
    {
        protected IIdentityClaimTypeRepository IdentityClaimTypeRepository { get; }
        public IdentityServerClaimTypeAppService(IIdentityClaimTypeRepository identityClaimTypeRepository)
        {
            IdentityClaimTypeRepository = identityClaimTypeRepository;
        }

        /// <summary>
        /// Get all claim-types
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<IdentityClaimTypeDto>> GetAllAsync()
        {
            var identityClaimTypes = await IdentityClaimTypeRepository.GetListAsync(false, default);
            return ObjectMapper.Map<List<IdentityClaimType>, List<IdentityClaimTypeDto>>(identityClaimTypes);
        }
    }
}
