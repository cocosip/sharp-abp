using Microsoft.AspNetCore.Mvc;
using SharpAbp.Abp.IdentityServer.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer
{
    [RemoteService(Name = IdentityServerRemoteServiceConsts.RemoteServiceName)]
    [Area("identity-server")]
    [Route("api/identity-server/clients")]
    public class IdentityServerClientController : IdentityServerController, IIdentityServerClientAppService
    {
        private readonly IIdentityServerClientAppService _identityServerClientAppService;
        public IdentityServerClientController(IIdentityServerClientAppService identityServerClientAppService)
        {
            _identityServerClientAppService = identityServerClientAppService;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ClientDto> GetAsync(Guid id)
        {
            return await _identityServerClientAppService.GetAsync(id);
        }

        /// <summary>
        /// Find by clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find-by-clientId/{clientId}")]
        public async Task<ClientDto> FindByClientIdAsync(string clientId)
        {
            return await _identityServerClientAppService.FindByClientIdAsync(clientId);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagedResultDto<ClientDto>> GetPagedListAsync(ClientPagedRequestDto input)
        {
            return await _identityServerClientAppService.GetPagedListAsync(input);
        }

        /// <summary>
        /// Get all distinct allowedCors origins
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("allowed-cors-origins")]
        public async Task<List<string>> GetAllDistinctAllowedCorsOriginsAsync()
        {
            return await _identityServerClientAppService.GetAllDistinctAllowedCorsOriginsAsync();
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Guid> CreateAsync(CreateClientDto input)
        {
            return await _identityServerClientAppService.CreateAsync(input);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateClientDto input)
        {
            await _identityServerClientAppService.UpdateAsync(id, input);
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
            await _identityServerClientAppService.DeleteAsync(id);
        }
    }
}
