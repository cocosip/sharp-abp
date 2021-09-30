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

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateIdentityClaimTypeDto input);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, UpdateIdentityClaimTypeDto input);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

    }
}
