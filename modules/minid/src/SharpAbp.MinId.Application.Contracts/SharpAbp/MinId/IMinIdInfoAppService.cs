﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Application service interface for managing MinId information.
    /// This interface provides methods for CRUD operations on MinIdInfo entities.
    /// </summary>
    public interface IMinIdInfoAppService : IApplicationService
    {
        /// <summary>
        /// Gets a MinIdInfo entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity.</param>
        /// <returns>The MinIdInfoDto representing the requested entity.</returns>
        Task<MinIdInfoDto> GetAsync(Guid id);

        /// <summary>
        /// Finds a MinIdInfo entity by its business type.
        /// </summary>
        /// <param name="bizType">The business type of the MinIdInfo entity.</param>
        /// <returns>The MinIdInfoDto representing the found entity, or null if not found.</returns>
        Task<MinIdInfoDto> FindByBizTypeAsync(string bizType);

        /// <summary>
        /// Gets a paged list of MinIdInfo entities based on the provided request parameters.
        /// </summary>
        /// <param name="input">The paged request DTO containing filtering and paging parameters.</param>
        /// <returns>A paged result containing the MinIdInfoDto entities.</returns>
        Task<PagedResultDto<MinIdInfoDto>> GetPagedListAsync(MinIdInfoPagedRequestDto input);

        /// <summary>
        /// Gets a list of MinIdInfo entities with optional sorting and filtering by business type.
        /// </summary>
        /// <param name="sorting">The sorting criteria for the results.</param>
        /// <param name="bizType">The business type to filter by (optional).</param>
        /// <returns>A list of MinIdInfoDto entities.</returns>
        Task<List<MinIdInfoDto>> GetListAsync(string sorting = null, string bizType = "");

        /// <summary>
        /// Creates a new MinIdInfo entity based on the provided input data.
        /// </summary>
        /// <param name="input">The DTO containing the data for the new MinIdInfo entity.</param>
        /// <returns>The created MinIdInfoDto representing the new entity.</returns>
        Task<MinIdInfoDto> CreateAsync(CreateMinIdInfoDto input);

        /// <summary>
        /// Updates an existing MinIdInfo entity with the specified ID using the provided input data.
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity to update.</param>
        /// <param name="input">The DTO containing the updated data for the MinIdInfo entity.</param>
        /// <returns>The updated MinIdInfoDto representing the modified entity.</returns>
        Task<MinIdInfoDto> UpdateAsync(Guid id, UpdateMinIdInfoDto input);

        /// <summary>
        /// Deletes a MinIdInfo entity with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteAsync(Guid id);
    }
}