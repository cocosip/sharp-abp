﻿﻿﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    [Authorize(MinIdPermissions.MinIdTokens.Default)]
    public class MinIdTokenAppService : MinIdAppService, IMinIdTokenAppService
    {
        protected IMinIdTokenManager MinIdTokenManager { get; }
        protected IMinIdTokenRepository MinIdTokenRepository { get; }

        public MinIdTokenAppService(
            IMinIdTokenManager minIdTokenManager,
            IMinIdTokenRepository minIdTokenRepository)
        {
            MinIdTokenManager = minIdTokenManager;
            MinIdTokenRepository = minIdTokenRepository;
        }

        /// <summary>
        /// Get a MinId token by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token.</param>
        /// <returns>The MinId token data transfer object.</returns>
        [Authorize(MinIdPermissions.MinIdTokens.Default)]
        public virtual async Task<MinIdTokenDto> GetAsync(Guid id)
        {
            var minIdToken = await MinIdTokenRepository.GetAsync(id);
            return ObjectMapper.Map<MinIdToken, MinIdTokenDto>(minIdToken);
        }

        /// <summary>
        /// Find a MinId token by business type and token value.
        /// </summary>
        /// <param name="bizType">The business type of the token.</param>
        /// <param name="token">The token value.</param>
        /// <returns>The MinId token data transfer object.</returns>
        [Authorize(MinIdPermissions.MinIdTokens.Default)]
        public virtual async Task<MinIdTokenDto> FindByTokenAsync(string bizType, string token)
        {
            var minIdToken = await MinIdTokenRepository.FindByTokenAsync(bizType, token);
            return ObjectMapper.Map<MinIdToken, MinIdTokenDto>(minIdToken);
        }

        /// <summary>
        /// Get a paged list of MinId tokens based on the provided filter.
        /// </summary>
        /// <param name="input">The paged request input containing filter and pagination parameters.</param>
        /// <returns>A paged result containing MinId token data transfer objects.</returns>
        [Authorize(MinIdPermissions.MinIdTokens.Default)]
        public virtual async Task<PagedResultDto<MinIdTokenDto>> GetPagedListAsync(MinIdTokenPagedRequestDto input)
        {
            var count = await MinIdTokenRepository.GetCountAsync(input.BizType, input.Token);
            var minIdTokens = await MinIdTokenRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.BizType,
                input.Token);

            return new PagedResultDto<MinIdTokenDto>(
                count,
                ObjectMapper.Map<List<MinIdToken>, List<MinIdTokenDto>>(minIdTokens)
                );
        }

        /// <summary>
        /// Create a new MinId token with the provided information.
        /// </summary>
        /// <param name="input">The creation input containing token details.</param>
        /// <returns>The created MinId token data transfer object.</returns>
        [Authorize(MinIdPermissions.MinIdTokens.Create)]
        public virtual async Task<MinIdTokenDto> CreateAsync(CreateMinIdTokenDto input)
        {
            var minIdToken = new MinIdToken(
                GuidGenerator.Create(),
                input.BizType,
                input.Token,
                input.Remark);

            await MinIdTokenManager.CreateAsync(minIdToken);
            return ObjectMapper.Map<MinIdToken, MinIdTokenDto>(minIdToken);
        }

        /// <summary>
        /// Update an existing MinId token with the provided information.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token to update.</param>
        /// <param name="input">The update input containing new token details.</param>
        /// <returns>The updated MinId token data transfer object.</returns>
        [Authorize(MinIdPermissions.MinIdTokens.Update)]
        public virtual async Task<MinIdTokenDto> UpdateAsync(Guid id, UpdateMinIdTokenDto input)
        {
            var minIdToken = await MinIdTokenManager.UpdateAsync(id, input.BizType, input.Token, input.Remark);
            return ObjectMapper.Map<MinIdToken, MinIdTokenDto>(minIdToken);
        }

        /// <summary>
        /// Delete a MinId token by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId token to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        [Authorize(MinIdPermissions.MinIdTokens.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await MinIdTokenRepository.DeleteAsync(id);
        }

    }
}