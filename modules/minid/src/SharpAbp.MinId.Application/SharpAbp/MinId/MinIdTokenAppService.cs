using Microsoft.AspNetCore.Authorization;
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
        /// Get minIdToken by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdTokens.Default)]
        public virtual async Task<MinIdTokenDto> GetAsync(Guid id)
        {
            var minIdToken = await MinIdTokenRepository.GetAsync(id);
            return ObjectMapper.Map<MinIdToken, MinIdTokenDto>(minIdToken);
        }

        /// <summary>
        /// Find minIdToken by token
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdTokens.Default)]
        public virtual async Task<MinIdTokenDto> FindByTokenAsync(string bizType, string token)
        {
            var minIdToken = await MinIdTokenRepository.FindByTokenAsync(bizType, token);
            return ObjectMapper.Map<MinIdToken, MinIdTokenDto>(minIdToken);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdTokens.Default)]
        public virtual async Task<PagedResultDto<MinIdTokenDto>> GetPagedListAsync(MinIdTokenPagedRequestDto input)
        {
            var count = await MinIdTokenRepository.GetCountAsync(input.BizType, input.Token);
            var minIdTokens = await MinIdTokenRepository.GetListAsync(
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
        /// Create minIdToken
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdTokens.Create)]
        public virtual async Task<Guid> CreateAsync(CreateMinIdTokenDto input)
        {
            var minIdToken = new MinIdToken(
                GuidGenerator.Create(),
                input.BizType,
                input.Token,
                input.Remark);

            await MinIdTokenManager.CreateAsync(minIdToken);
            return minIdToken.Id;
        }

        /// <summary>
        /// Update minIdToken
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdTokens.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateMinIdTokenDto input)
        {
            await MinIdTokenManager.UpdateAsync(id, input.BizType, input.Token, input.Remark);
        }

        /// <summary>
        /// Delete minIdToken
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdTokens.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await MinIdTokenRepository.DeleteAsync(id);
        }

    }
}
