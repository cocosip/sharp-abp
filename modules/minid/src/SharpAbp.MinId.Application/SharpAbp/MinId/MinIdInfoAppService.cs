﻿﻿﻿using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Application service implementation for managing MinId information.
    /// This service provides methods for CRUD operations on MinIdInfo entities.
    /// </summary>
    [Authorize(MinIdPermissions.MinIdInfos.Default)]
    public class MinIdInfoAppService : MinIdAppService, IMinIdInfoAppService
    {
        /// <summary>
        /// The manager responsible for handling MinIdInfo business logic.
        /// </summary>
        protected IMinIdInfoManager MinIdInfoManager { get; }

        /// <summary>
        /// The repository for accessing MinIdInfo data.
        /// </summary>
        protected IMinIdInfoRepository MinIdInfoRepository { get; }

        /// <summary>
        /// Initializes a new instance of the MinIdInfoAppService class.
        /// </summary>
        /// <param name="minIdInfoManager">The manager for MinIdInfo business logic.</param>
        /// <param name="minIdInfoRepository">The repository for MinIdInfo data access.</param>
        public MinIdInfoAppService(
            IMinIdInfoManager minIdInfoManager,
            IMinIdInfoRepository minIdInfoRepository)
        {
            MinIdInfoManager = minIdInfoManager;
            MinIdInfoRepository = minIdInfoRepository;
        }

        /// <summary>
        /// Gets a MinIdInfo entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity.</param>
        /// <returns>The MinIdInfoDto representing the requested entity.</returns>
        [Authorize(MinIdPermissions.MinIdInfos.Default)]
        public virtual async Task<MinIdInfoDto> GetAsync(Guid id)
        {
            var minIdInfo = await MinIdInfoRepository.GetAsync(id);
            return ObjectMapper.Map<MinIdInfo, MinIdInfoDto>(minIdInfo);
        }

        /// <summary>
        /// Finds a MinIdInfo entity by its business type.
        /// </summary>
        /// <param name="bizType">The business type of the MinIdInfo entity. Cannot be null or white space.</param>
        /// <returns>The MinIdInfoDto representing the found entity.</returns>
        [Authorize(MinIdPermissions.MinIdInfos.Default)]
        public virtual async Task<MinIdInfoDto> FindByBizTypeAsync([NotNull] string bizType)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType);
            return ObjectMapper.Map<MinIdInfo, MinIdInfoDto>(minIdInfo);
        }

        /// <summary>
        /// Gets a paged list of MinIdInfo entities based on the provided request parameters.
        /// </summary>
        /// <param name="input">The paged request DTO containing filtering and paging parameters.</param>
        /// <returns>A paged result containing the MinIdInfoDto entities.</returns>
        [Authorize(MinIdPermissions.MinIdInfos.Default)]
        public virtual async Task<PagedResultDto<MinIdInfoDto>> GetPagedListAsync(MinIdInfoPagedRequestDto input)
        {
            var count = await MinIdInfoRepository.GetCountAsync(input.BizType);
            var minIdInfos = await MinIdInfoRepository.GetPagedListAsync(
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
        /// Gets a list of MinIdInfo entities with optional sorting and filtering by business type.
        /// </summary>
        /// <param name="sorting">The sorting criteria for the results.</param>
        /// <param name="bizType">The business type to filter by (optional).</param>
        /// <returns>A list of MinIdInfoDto entities.</returns>
        [Authorize(MinIdPermissions.MinIdInfos.Default)]
        public virtual async Task<List<MinIdInfoDto>> GetListAsync(string sorting = null, string bizType = "")
        {
            var minIdInfos = await MinIdInfoRepository.GetListAsync(sorting, bizType);
            return ObjectMapper.Map<List<MinIdInfo>, List<MinIdInfoDto>>(minIdInfos);
        }

        /// <summary>
        /// Creates a new MinIdInfo entity based on the provided input data.
        /// </summary>
        /// <param name="input">The DTO containing the data for the new MinIdInfo entity.</param>
        /// <returns>The created MinIdInfoDto representing the new entity.</returns>
        [Authorize(MinIdPermissions.MinIdInfos.Create)]
        public virtual async Task<MinIdInfoDto> CreateAsync(CreateMinIdInfoDto input)
        {
            var minIdInfo = new MinIdInfo(
                GuidGenerator.Create(),
                input.BizType,
                input.MaxId,
                input.Step,
                input.Delta,
                input.Remainder);

            await MinIdInfoManager.CreateAsync(minIdInfo);
            return ObjectMapper.Map<MinIdInfo, MinIdInfoDto>(minIdInfo);
        }

        /// <summary>
        /// Updates an existing MinIdInfo entity with the specified ID using the provided input data.
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity to update.</param>
        /// <param name="input">The DTO containing the updated data for the MinIdInfo entity.</param>
        /// <returns>The updated MinIdInfoDto representing the modified entity.</returns>
        [Authorize(MinIdPermissions.MinIdInfos.Update)]
        public virtual async Task<MinIdInfoDto> UpdateAsync(Guid id, UpdateMinIdInfoDto input)
        {
            var minIdInfo = await MinIdInfoManager.UpdateAsync(
                id,
                input.BizType,
                input.MaxId,
                input.Step,
                input.Delta,
                input.Remainder);

            return ObjectMapper.Map<MinIdInfo, MinIdInfoDto>(minIdInfo);
        }

        /// <summary>
        /// Deletes a MinIdInfo entity with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        [Authorize(MinIdPermissions.MinIdInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await MinIdInfoRepository.DeleteAsync(id);
        }
    }
}