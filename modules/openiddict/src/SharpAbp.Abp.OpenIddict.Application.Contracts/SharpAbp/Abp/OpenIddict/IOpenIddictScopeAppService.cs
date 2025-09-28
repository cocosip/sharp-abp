using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.OpenIddict
{
    /// <summary>
    /// Application service interface for managing OpenIddict scopes
    /// </summary>
    public interface IOpenIddictScopeAppService : IApplicationService
    {
        /// <summary>
        /// Gets an OpenIddict scope by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the scope</param>
        /// <returns>The OpenIddict scope DTO</returns>
        Task<OpenIddictScopeDto> GetAsync(Guid id);
        /// <summary>
        /// Finds an OpenIddict scope by its name
        /// </summary>
        /// <param name="name">The name of the scope</param>
        /// <returns>The OpenIddict scope DTO if found, null otherwise</returns>
        Task<OpenIddictScopeDto> FindByNameAsync(string name);
        /// <summary>
        /// Gets a paged list of OpenIddict scopes
        /// </summary>
        /// <param name="input">The paged request parameters</param>
        /// <returns>A paged result of OpenIddict scope DTOs</returns>
        Task<PagedResultDto<OpenIddictScopeDto>> GetPagedListAsync(OpenIddictScopePagedRequestDto input);
        /// <summary>
        /// Gets all OpenIddict scopes
        /// </summary>
        /// <returns>A list of all OpenIddict scope DTOs</returns>
        Task<List<OpenIddictScopeDto>> GetListAsync();
        /// <summary>
        /// Creates a new OpenIddict scope
        /// </summary>
        /// <param name="input">The scope creation data</param>
        /// <returns>The created OpenIddict scope DTO</returns>
        Task<OpenIddictScopeDto> CreateAsync(CreateOpenIddictScopeDto input);
        /// <summary>
        /// Updates an existing OpenIddict scope
        /// </summary>
        /// <param name="id">The unique identifier of the scope to update</param>
        /// <param name="input">The scope update data</param>
        /// <returns>The updated OpenIddict scope DTO</returns>
        Task<OpenIddictScopeDto> UpdateAsync(Guid id, UpdateOpenIddictScopeDto input);
        /// <summary>
        /// Deletes an OpenIddict scope
        /// </summary>
        /// <param name="id">The unique identifier of the scope to delete</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task DeleteAsync(Guid id);
    }
}
