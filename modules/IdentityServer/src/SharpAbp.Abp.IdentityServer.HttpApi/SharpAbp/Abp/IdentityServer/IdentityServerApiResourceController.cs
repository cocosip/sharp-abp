using Microsoft.AspNetCore.Mvc;
using SharpAbp.Abp.IdentityServer.ApiResources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer
{
    [RemoteService(Name = IdentityServerRemoteServiceConsts.RemoteServiceName)]
    [Area("identity-server")]
    [Route("api/identity-server/api-resources")]
    public class IdentityServerApiResourceController : IdentityServerController, IIdentityServerApiResourceAppService
    {

        private readonly IIdentityServerApiResourceAppService _identityServerApiResourceAppService;
        public IdentityServerApiResourceController(IIdentityServerApiResourceAppService identityServerApiResourceAppService)
        {
            _identityServerApiResourceAppService = identityServerApiResourceAppService;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ApiResourceDto> GetAsync(Guid id)
        {
            return await _identityServerApiResourceAppService.GetAsync(id);
        }

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="apiResourceName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find-by-name/{apiResourceName}")]
        public async Task<ApiResourceDto> FindByNameAsync(string apiResourceName)
        {
            return await _identityServerApiResourceAppService.FindByNameAsync(apiResourceName);
        }

        /// <summary>
        /// Get list by scopeNames
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-list")]
        public async Task<List<ApiResourceDto>> GetListByScopesAsync(string[] scopeNames)
        {
            return await _identityServerApiResourceAppService.GetListByScopesAsync(scopeNames);
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<ApiResourceDto>> GetAllAsync()
        {
            return await _identityServerApiResourceAppService.GetAllAsync();
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagedResultDto<ApiResourceDto>> GetPagedListAsync(ApiResourcePagedRequestDto input)
        {
            return await _identityServerApiResourceAppService.GetPagedListAsync(input);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Guid> CreateAsync(CreateApiResourceDto input)
        {
            return await _identityServerApiResourceAppService.CreateAsync(input);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateApiResourceDto input)
        {
            await _identityServerApiResourceAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _identityServerApiResourceAppService.DeleteAsync(id);
        }


    }
}
