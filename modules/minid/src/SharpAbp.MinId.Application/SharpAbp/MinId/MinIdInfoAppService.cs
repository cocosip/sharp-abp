using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    [Authorize(MinIdPermissions.MinIdInfos.Default)]
    public class MinIdInfoAppService : MinIdAppService, IMinIdInfoAppService
    {
        protected IMinIdInfoManager MinIdInfoManager { get; }
        protected IMinIdInfoRepository MinIdInfoRepository { get; }

        public MinIdInfoAppService(
            IMinIdInfoManager minIdInfoManager,
            IMinIdInfoRepository minIdInfoRepository)
        {
            MinIdInfoManager = minIdInfoManager;
            MinIdInfoRepository = minIdInfoRepository;
        }

        /// <summary>
        /// Get minIdInfo by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdInfos.Default)]
        public virtual async Task<MinIdInfoDto> GetAsync(Guid id)
        {
            var minIdInfo = await MinIdInfoRepository.GetAsync(id);
            return ObjectMapper.Map<MinIdInfo, MinIdInfoDto>(minIdInfo);
        }

        /// <summary>
        /// Find minIdInfo by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdInfos.Default)]
        public virtual async Task<MinIdInfoDto> FindByBizTypeAsync([NotNull] string bizType)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType);
            return ObjectMapper.Map<MinIdInfo, MinIdInfoDto>(minIdInfo);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdInfos.Default)]
        public virtual async Task<PagedResultDto<MinIdInfoDto>> GetPagedListAsync(MinIdInfoPagedRequestDto input)
        {
            var count = await MinIdInfoRepository.GetCountAsync(input.BizType);
            var minIdInfos = await MinIdInfoRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.BizType);

            return new PagedResultDto<MinIdInfoDto>(
                count,
                ObjectMapper.Map<List<MinIdInfo>, List<MinIdInfoDto>>(minIdInfos)
                );
        }

        /// <summary>
        /// Create minIdInfo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdInfos.Create)]
        public virtual async Task<Guid> CreateAsync(CreateMinIdInfoDto input)
        {
            var minIdInfo = new MinIdInfo(
                GuidGenerator.Create(),
                input.BizType,
                input.MaxId,
                input.Step,
                input.Delta,
                input.Remainder);

            await MinIdInfoManager.CreateAsync(minIdInfo);
            return minIdInfo.Id;
        }

        /// <summary>
        /// Update minIdInfo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdInfos.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateMinIdInfoDto input)
        {
            await MinIdInfoManager.UpdateAsync(
                id, 
                input.BizType,
                input.MaxId, 
                input.Step, 
                input.Delta,
                input.Remainder);
        }

        /// <summary>
        /// Delete minIdInfo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(MinIdPermissions.MinIdInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await MinIdInfoRepository.DeleteAsync(id);
        }


    }
}
