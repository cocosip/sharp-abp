﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    /// <summary>
    /// REST controller for managing MinId tokens.
    /// </summary>
    [RemoteService(Name = MinIdRemoteServiceConsts.RemoteServiceName)]
    [Area("minid")]
    [Route("api/minid/minid-token")]
    public class MinIdTokenController : MinIdController, IMinIdTokenAppService
    {
        private readonly IMinIdTokenAppService _minIdTokenAppService;

        /// <summary>
        /// Constructor for MinIdTokenController.
        /// </summary>
        /// <param name="minIdTokenAppService">The MinId token application service.</param>
        public MinIdTokenController(IMinIdTokenAppService minIdTokenAppService)
        {
            _minIdTokenAppService = minIdTokenAppService;
        }

        /// <summary>
        /// Get a MinId token by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token.</param>
        /// <returns>The MinId token data transfer object.</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<MinIdTokenDto> GetAsync(Guid id)
        {
            return await _minIdTokenAppService.GetAsync(id);
        }

        /// <summary>
        /// Find a MinId token by business type and token value.
        /// </summary>
        /// <param name="bizType">The business type of the token.</param>
        /// <param name="token">The token value.</param>
        /// <returns>The MinId token data transfer object.</returns>
        [HttpGet]
        [Route("find-by-token/{bizType}/{token}")]
        public async Task<MinIdTokenDto> FindByTokenAsync(string bizType, string token)
        {
            return await _minIdTokenAppService.FindByTokenAsync(bizType, token);
        }

        /// <summary>
        /// Get a paged list of MinId tokens based on the provided filter.
        /// </summary>
        /// <param name="input">The paged request input containing filter and pagination parameters.</param>
        /// <returns>A paged result containing MinId token data transfer objects.</returns>
        [HttpGet]
        public async Task<PagedResultDto<MinIdTokenDto>> GetPagedListAsync(MinIdTokenPagedRequestDto input)
        {
            return await _minIdTokenAppService.GetPagedListAsync(input);
        }

        /// <summary>
        /// Create a new MinId token with the provided information.
        /// </summary>
        /// <param name="input">The creation input containing token details.</param>
        /// <returns>The created MinId token data transfer object.</returns>
        [HttpPost]
        public async Task<MinIdTokenDto> CreateAsync(CreateMinIdTokenDto input)
        {
            return await _minIdTokenAppService.CreateAsync(input);
        }

        /// <summary>
        /// Update an existing MinId token with the provided information.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token to update.</param>
        /// <param name="input">The update input containing new token details.</param>
        /// <returns>The updated MinId token data transfer object.</returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<MinIdTokenDto> UpdateAsync(Guid id, UpdateMinIdTokenDto input)
        {
            return await _minIdTokenAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Delete a MinId token by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _minIdTokenAppService.DeleteAsync(id);
        }
    }
}