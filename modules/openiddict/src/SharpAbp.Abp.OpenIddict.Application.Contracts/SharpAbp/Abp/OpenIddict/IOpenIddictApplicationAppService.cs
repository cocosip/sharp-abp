using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.OpenIddict
{
    /// <summary>
    /// Application service interface for managing OpenIddict applications
    /// </summary>
    public interface IOpenIddictApplicationAppService : IApplicationService
    {
        /// <summary>
        /// Gets an OpenIddict application by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the application</param>
        /// <returns>The OpenIddict application DTO</returns>
        Task<OpenIddictApplicationDto> GetAsync(Guid id);
        
        /// <summary>
        /// Finds an OpenIddict application by its client identifier
        /// </summary>
        /// <param name="clientId">The client identifier of the application</param>
        /// <returns>The OpenIddict application DTO</returns>
        Task<OpenIddictApplicationDto> FindByClientIdAsync(string clientId);
        
        /// <summary>
        /// Gets a paged list of OpenIddict applications
        /// </summary>
        /// <param name="input">The paged request parameters</param>
        /// <returns>A paged result containing OpenIddict application DTOs</returns>
        Task<PagedResultDto<OpenIddictApplicationDto>> GetPagedListAsync(OpenIddictApplicationPagedRequestDto input);
        
        /// <summary>
        /// Gets all OpenIddict applications
        /// </summary>
        /// <returns>A list of OpenIddict application DTOs</returns>
        Task<List<OpenIddictApplicationDto>> GetListAsync();
        
        /// <summary>
        /// Creates a new OpenIddict application
        /// </summary>
        /// <param name="input">The application creation data</param>
        /// <returns>The created OpenIddict application DTO</returns>
        Task<OpenIddictApplicationDto> CreateAsync(CreateOrUpdateOpenIddictApplicationDto input);
        
        /// <summary>
        /// Updates an existing OpenIddict application
        /// </summary>
        /// <param name="id">The unique identifier of the application to update</param>
        /// <param name="input">The application update data</param>
        /// <returns>The updated OpenIddict application DTO</returns>
        Task<OpenIddictApplicationDto> UpdateAsync(Guid id, CreateOrUpdateOpenIddictApplicationDto input);
        
        /// <summary>
        /// Deletes an OpenIddict application
        /// </summary>
        /// <param name="id">The unique identifier of the application to delete</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task DeleteAsync(Guid id);
    }
}
