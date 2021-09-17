using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.Clients;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    [Authorize(IdentityServerPermissions.Clients.Default)]
    public class ClientAppService : IdentityServerAppServiceBase, IClientAppService
    {
        protected IClientRepository ClientRepository { get; }
        public ClientAppService(IClientRepository clientRepository)
        {
            ClientRepository = clientRepository;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.Clients.Default)]
        public virtual async Task<ClientDto> GetAsync(Guid id)
        {
            var client = await ClientRepository.GetAsync(id);
            return ObjectMapper.Map<Client, ClientDto>(client);
        }

        /// <summary>
        /// Find by clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.Clients.Default)]
        public virtual async Task<ClientDto> FindByClientIdAsync(string clientId)
        {
            var client = await ClientRepository.FindByClientIdAsync(clientId);
            return ObjectMapper.Map<Client, ClientDto>(client);
        }

        /// <summary>
        /// Client paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.Clients.Default)]
        public virtual async Task<PagedResultDto<ClientDto>> GetPagedListAsync(ClientPagedRequestDto input)
        {
            var count = await ClientRepository.GetCountAsync(input.Filter);
            var clients = await ClientRepository.GetListAsync(
                input.Sorting,
                input.SkipCount,
                input.MaxResultCount,
                input.Filter);

            return new PagedResultDto<ClientDto>(
              count,
              ObjectMapper.Map<List<Client>, List<ClientDto>>(clients)
              );
        }

        /// <summary>
        /// Get all distinct allowed cors origins
        /// </summary>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.Clients.Default)]
        public virtual async Task<List<string>> GetAllDistinctAllowedCorsOriginsAsync()
        {
            return await ClientRepository.GetAllDistinctAllowedCorsOriginsAsync();
        }
    }
}
