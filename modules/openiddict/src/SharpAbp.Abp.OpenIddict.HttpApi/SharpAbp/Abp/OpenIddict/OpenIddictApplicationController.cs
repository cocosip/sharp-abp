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
    [Route("api/openiddict/applications")]
    public class OpenIddictApplicationController : OpenIddictController, IOpenIddictApplicationAppService
    {
        private readonly IOpenIddictApplicationAppService _openIddictApplicationAppService;
        public OpenIddictApplicationController(IOpenIddictApplicationAppService openIddictApplicationAppService)
        {
            _openIddictApplicationAppService = openIddictApplicationAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<OpenIddictApplicationDto> GetAsync(Guid id)
        {
            return await _openIddictApplicationAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("list")]
        public async Task<List<OpenIddictApplicationDto>> GetListAsync()
        {
            return await _openIddictApplicationAppService.GetListAsync();
        }

        [HttpGet]
        public async Task<PagedResultDto<OpenIddictApplicationDto>> GetPagedListAsync(OpenIddictApplicationPagedRequestDto input)
        {
            return await _openIddictApplicationAppService.GetPagedListAsync(input);
        }

        [HttpGet]
        [Route("find-by-clientid/{clientId}")]
        public async Task<OpenIddictApplicationDto> FindByClientIdAsync(string clientId)
        {
            return await _openIddictApplicationAppService.FindByClientIdAsync(clientId);
        }

        [HttpPost]
        public async Task<OpenIddictApplicationDto> CreateAsync(CreateOrUpdateOpenIddictApplicationDto input)
        {
            return await _openIddictApplicationAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<OpenIddictApplicationDto> UpdateAsync(Guid id, CreateOrUpdateOpenIddictApplicationDto input)
        {
            return await _openIddictApplicationAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _openIddictApplicationAppService.DeleteAsync(id);
        }
    }
}
