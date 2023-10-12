using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.OpenIddict
{
    [RemoteService(Name = OpenIddictRemoteServiceConsts.RemoteServiceName)]
    [Area("openiddict")]
    [Route("api/openiddict/scopes")]
    public class OpenIddictScopeController : OpenIddictController, IOpenIddictScopeAppService
    {
        private readonly IOpenIddictScopeAppService _openIddictScopeAppService;

        public OpenIddictScopeController(IOpenIddictScopeAppService openIddictScopeAppService)
        {
            _openIddictScopeAppService = openIddictScopeAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<OpenIddictScopeDto> GetAsync(Guid id)
        {
            return await _openIddictScopeAppService.GetAsync(id);
        }

        [HttpGet]
        public async Task<PagedResultDto<OpenIddictScopeDto>> GetPagedListAsync(OpenIddictScopePagedRequestDto input)
        {
            return await _openIddictScopeAppService.GetPagedListAsync(input);
        }

        [HttpGet]
        [Route("list")]
        public async Task<List<OpenIddictScopeDto>> GetListAsync()
        {
            return await _openIddictScopeAppService.GetListAsync();
        }

        [HttpGet]
        [Route("find-by-name/{name}")]
        public async Task<OpenIddictScopeDto> FindByNameAsync(string name)
        {
            return await _openIddictScopeAppService.FindByNameAsync(name);
        }

        [HttpPost]
        public async Task<OpenIddictScopeDto> CreateAsync(CreateOpenIddictScopeDto input)
        {
            return await _openIddictScopeAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<OpenIddictScopeDto> UpdateAsync(Guid id, UpdateOpenIddictScopeDto input)
        {
            return await _openIddictScopeAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _openIddictScopeAppService.DeleteAsync(id);
        }
    }
}
