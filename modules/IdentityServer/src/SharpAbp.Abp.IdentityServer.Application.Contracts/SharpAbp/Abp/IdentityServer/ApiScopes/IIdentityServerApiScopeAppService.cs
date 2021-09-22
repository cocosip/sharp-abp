using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.IdentityServer.ApiScopes
{
    public interface IIdentityServerApiScopeAppService : IApplicationService
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ApiScopeDto> GetAsync(Guid id);

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<ApiScopeDto> FindByName(string name);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ApiScopeDto>> GetPagedListAsync(ApiScopePagedRequestDto input);

        /// <summary>
        /// Get list by scope names
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        Task<List<ApiScopeDto>> GetListByNameAsync(string[] scopeNames);

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateApiScopeDto input);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, UpdateApiScopeDto input);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

    }
}
