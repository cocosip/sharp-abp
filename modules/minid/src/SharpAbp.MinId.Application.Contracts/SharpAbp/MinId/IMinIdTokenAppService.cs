﻿using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    public interface IMinIdTokenAppService : IApplicationService
    {
        /// <summary>
        /// Get a MinId token by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token.</param>
        /// <returns>The MinId token data transfer object.</returns>
        Task<MinIdTokenDto> GetAsync(Guid id);

        /// <summary>
        /// Find a MinId token by business type and token value.
        /// </summary>
        /// <param name="bizType">The business type of the token.</param>
        /// <param name="token">The token value.</param>
        /// <returns>The MinId token data transfer object.</returns>
        Task<MinIdTokenDto> FindByTokenAsync(string bizType, string token);

        /// <summary>
        /// Get a paged list of MinId tokens based on the provided filter.
        /// </summary>
        /// <param name="input">The paged request input containing filter and pagination parameters.</param>
        /// <returns>A paged result containing MinId token data transfer objects.</returns>
        Task<PagedResultDto<MinIdTokenDto>> GetPagedListAsync(MinIdTokenPagedRequestDto input);

        /// <summary>
        /// Create a new MinId token with the provided information.
        /// </summary>
        /// <param name="input">The creation input containing token details.</param>
        /// <returns>The created MinId token data transfer object.</returns>
        Task<MinIdTokenDto> CreateAsync(CreateMinIdTokenDto input);

        /// <summary>
        /// Update an existing MinId token with the provided information.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token to update.</param>
        /// <param name="input">The update input containing new token details.</param>
        /// <returns>The updated MinId token data transfer object.</returns>
        Task<MinIdTokenDto> UpdateAsync(Guid id, UpdateMinIdTokenDto input);

        /// <summary>
        /// Delete a MinId token by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteAsync(Guid id);
    }
}