using Microsoft.AspNetCore.Mvc;
using SharpAbp.Abp.IdentityServer.IdentityResources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer
{
    [RemoteService(Name = IdentityServerRemoteServiceConsts.RemoteServiceName)]
    [Area("identity-server")]
    [Route("api/identity-server/identity-resources")]
    public class IdentityResourceController : IdentityServerController, IIdentityResourceAppService
    {
        private readonly IIdentityResourceAppService _identityResourceAppService;
        public IdentityResourceController(IIdentityResourceAppService identityResourceAppService)
        {
            _identityResourceAppService = identityResourceAppService;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IdentityResourceDto> GetAsync(Guid id)
        {
            return await _identityResourceAppService.GetAsync(id);
        }

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find-by-name/{name}")]
        public async Task<IdentityResourceDto> FindByNameAsync(string name)
        {
            return await _identityResourceAppService.FindByNameAsync(name);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagedResultDto<IdentityResourceDto>> GetPagedListAsync(IdentityResourcePagedRequestDto input)
        {
            return await _identityResourceAppService.GetPagedListAsync(input);
        }

        /// <summary>
        /// Get list by scopeNames
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-list")]
        public async Task<List<IdentityResourceDto>> GetListByScopeNameAsync(string[] scopeNames)
        {
            return await _identityResourceAppService.GetListByScopeNameAsync(scopeNames);
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<List<IdentityResourceDto>> GetAllAsync()
        {
            return await _identityResourceAppService.GetAllAsync();
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Guid> CreateAsync(CreateIdentityResourceDto input)
        {
            return await _identityResourceAppService.CreateAsync(input);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateIdentityResourceDto input)
        {
            await _identityResourceAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Delete by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _identityResourceAppService.DeleteAsync(id);
        }

     
    }
}
