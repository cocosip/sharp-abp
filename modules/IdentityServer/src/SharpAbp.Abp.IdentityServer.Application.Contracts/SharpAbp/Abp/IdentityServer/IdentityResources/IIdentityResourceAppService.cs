using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.IdentityServer.IdentityResources
{
    public interface IIdentityResourceAppService : IApplicationService
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IdentityResourceDto> GetAsync(Guid id);

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IdentityResourceDto> FindByNameAsync(string name);

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        Task<List<IdentityResourceDto>> GetAllAsync();

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<IdentityResourceDto>> GetPagedListAsync(IdentityResourcePagedRequestDto input);

        /// <summary>
        /// Get list by scope name
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        Task<List<IdentityResourceDto>> GetListByScopeNameAsync(string[] scopeNames);

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateIdentityResourceDto input);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, UpdateIdentityResourceDto input);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);


    }
}
