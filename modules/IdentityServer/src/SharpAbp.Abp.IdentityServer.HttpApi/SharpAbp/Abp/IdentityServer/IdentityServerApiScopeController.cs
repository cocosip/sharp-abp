using Microsoft.AspNetCore.Mvc;
using SharpAbp.Abp.IdentityServer.ApiScopes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer
{
    [RemoteService(Name = IdentityServerRemoteServiceConsts.RemoteServiceName)]
    [Area("identity-server")]
    [Route("api/identity-server/apiScopes")]
    public class IdentityServerApiScopeController : IdentityServerController, IIdentityServerApiScopeAppService
    {
        private readonly IIdentityServerApiScopeAppService _identityServerApiScopeAppService;
        public IdentityServerApiScopeController(IIdentityServerApiScopeAppService identityServerApiScopeAppService)
        {
            _identityServerApiScopeAppService = identityServerApiScopeAppService;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ApiScopeDto> GetAsync(Guid id)
        {
            return await _identityServerApiScopeAppService.GetAsync(id);
        }

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find-by-name/{name}")]
        public async Task<ApiScopeDto> FindByNameAsync(string name)
        {
            return await _identityServerApiScopeAppService.FindByNameAsync(name);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagedResultDto<ApiScopeDto>> GetPagedListAsync(ApiScopePagedRequestDto input)
        {
            return await _identityServerApiScopeAppService.GetPagedListAsync(input);
        }

        /// <summary>
        /// Get list by name
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-list")]
        public async Task<List<ApiScopeDto>> GetListByNameAsync(string[] scopeNames)
        {
            return await _identityServerApiScopeAppService.GetListByNameAsync(scopeNames);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Guid> CreateAsync(CreateApiScopeDto input)
        {
            return await _identityServerApiScopeAppService.CreateAsync(input);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateApiScopeDto input)
        {
            await _identityServerApiScopeAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _identityServerApiScopeAppService.DeleteAsync(id);
        }

    }
}
