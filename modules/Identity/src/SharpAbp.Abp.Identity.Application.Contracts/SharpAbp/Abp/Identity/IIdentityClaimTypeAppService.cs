using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.Identity
{
    public interface IIdentityClaimTypeAppService : IApplicationService
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IdentityClaimTypeDto> GetAsync(Guid id);

        /// <summary>
        /// Any
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoredId"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(string name, Guid? ignoredId = null);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<IdentityClaimTypeDto>> GetPagedListAsync(IdentityClaimTypePagedRequestDto input);
    
    }
}
