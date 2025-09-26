﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    /// <summary>
    /// REST controller for managing MinId information.
    /// Provides API endpoints for CRUD operations on MinIdInfo entities.
    /// </summary>
    [RemoteService(Name = MinIdRemoteServiceConsts.RemoteServiceName)]
    [Area("minid")]
    [Route("api/minid/minid-info")]
    public class MinIdInfoController : MinIdController, IMinIdInfoAppService
    {
        /// <summary>
        /// The application service for MinIdInfo operations.
        /// </summary>
        private readonly IMinIdInfoAppService _minIdInfoAppService;

        /// <summary>
        /// Initializes a new instance of the MinIdInfoController class.
        /// </summary>
        /// <param name="minIdInfoAppService">The application service for MinIdInfo operations.</param>
        public MinIdInfoController(IMinIdInfoAppService minIdInfoAppService)
        {
            _minIdInfoAppService = minIdInfoAppService;
        }

        /// <summary>
        /// Gets a MinIdInfo entity by its unique identifier.
        /// Route: GET api/minid/minid-info/{id}
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity.</param>
        /// <returns>The MinIdInfoDto representing the requested entity.</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<MinIdInfoDto> GetAsync(Guid id)
        {
            return await _minIdInfoAppService.GetAsync(id);
        }

        /// <summary>
        /// Finds a MinIdInfo entity by its business type.
        /// Route: GET api/minid/minid-info/find-by-bizType/{bizType}
        /// </summary>
        /// <param name="bizType">The business type of the MinIdInfo entity.</param>
        /// <returns>The MinIdInfoDto representing the found entity.</returns>
        [HttpGet]
        [Route("find-by-bizType/{bizType}")]
        public async Task<MinIdInfoDto> FindByBizTypeAsync(string bizType)
        {
            return await _minIdInfoAppService.FindByBizTypeAsync(bizType);
        }

        /// <summary>
        /// Gets a paged list of MinIdInfo entities based on the provided request parameters.
        /// Route: GET api/minid/minid-info
        /// </summary>
        /// <param name="input">The paged request DTO containing filtering and paging parameters.</param>
        /// <returns>A paged result containing the MinIdInfoDto entities.</returns>
        [HttpGet]
        public async Task<PagedResultDto<MinIdInfoDto>> GetPagedListAsync(MinIdInfoPagedRequestDto input)
        {
            return await _minIdInfoAppService.GetPagedListAsync(input);
        }

        /// <summary>
        /// Gets a list of MinIdInfo entities with optional sorting and filtering by business type.
        /// Route: GET api/minid/minid-info/get-list
        /// </summary>
        /// <param name="sorting">The sorting criteria for the results.</param>
        /// <param name="bizType">The business type to filter by (optional).</param>
        /// <returns>A list of MinIdInfoDto entities.</returns>
        [HttpGet]
        [Route("get-list")]
        public async Task<List<MinIdInfoDto>> GetListAsync(string sorting = null, string bizType = "")
        {
            return await _minIdInfoAppService.GetListAsync(sorting, bizType);
        }

        /// <summary>
        /// Creates a new MinIdInfo entity based on the provided input data.
        /// Route: POST api/minid/minid-info
        /// </summary>
        /// <param name="input">The DTO containing the data for the new MinIdInfo entity.</param>
        /// <returns>The created MinIdInfoDto representing the new entity.</returns>
        [HttpPost]
        public async Task<MinIdInfoDto> CreateAsync(CreateMinIdInfoDto input)
        {
            return await _minIdInfoAppService.CreateAsync(input);
        }

        /// <summary>
        /// Updates an existing MinIdInfo entity with the specified ID using the provided input data.
        /// Route: PUT api/minid/minid-info/{id}
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity to update.</param>
        /// <param name="input">The DTO containing the updated data for the MinIdInfo entity.</param>
        /// <returns>The updated MinIdInfoDto representing the modified entity.</returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<MinIdInfoDto> UpdateAsync(Guid id, UpdateMinIdInfoDto input)
        {
            return await _minIdInfoAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Deletes a MinIdInfo entity with the specified ID.
        /// Route: DELETE api/minid/minid-info/{id}
        /// </summary>
        /// <param name="id">The unique identifier of the MinIdInfo entity to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _minIdInfoAppService.DeleteAsync(id);
        }
    }
}