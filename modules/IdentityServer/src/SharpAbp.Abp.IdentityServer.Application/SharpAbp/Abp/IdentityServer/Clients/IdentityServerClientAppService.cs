using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.Clients;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    [Authorize(IdentityServerPermissions.Clients.Default)]
    public class IdentityServerClientAppService : IdentityServerAppServiceBase, IIdentityServerClientAppService
    {
        protected IClientRepository ClientRepository { get; }
        protected IIdentityServerClientManager ClientManager { get; }
        public IdentityServerClientAppService(
            IClientRepository clientRepository,
            IIdentityServerClientManager clientManager)
        {
            ClientRepository = clientRepository;
            ClientManager = clientManager;
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

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.Clients.Create)]
        public virtual async Task<Guid> CreateAsync(CreateClientDto input)
        {
            //validate
            await ClientManager.ValidateClientIdAsync(input.ClientId);

            var client = new Client(GuidGenerator.Create(), input.ClientId)
            {
                ClientName = input.ClientName,
                ProtocolType = "oidc",
                Description = input.Description,
                ClientUri = input.ClientUri,
                LogoUri = input.LogoUri,
                RequireConsent = input.RequireConsent
            };

            //scopes
            if (input.Scopes != null)
            {
                foreach (var scope in input.Scopes)
                {
                    client.AddScope(scope);
                }
            }

            //secrets
            foreach (var secret in input.Secrets)
            {
                var value = secret.Type == "SharedSecret" ? secret.Value : IdentityServer4.Models.HashExtensions.Sha256(secret.Value);
                client.AddSecret(value, secret.Expiration, secret.Type, secret.Description);
            }

            if (!input.CallbackUrl.IsNullOrWhiteSpace())
            {
                client.AddRedirectUri(input.CallbackUrl);
            }

            if (!input.LogoutUrl.IsNullOrWhiteSpace())
            {
                client.AddPostLogoutRedirectUri(input.LogoutUrl);
            }

            await ClientRepository.InsertAsync(client);

            return client.Id;
        }

        [Authorize(IdentityServerPermissions.Clients.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateClientDto input)
        {
            var client = await ClientRepository.GetAsync(id);
            client.ClientName = input.ClientName;
            client.Description = input.Description;
            client.ClientUri = input.ClientUri;
            client.LogoUri = input.LogoUri;

            client.RequireConsent = input.RequireConsent;
            client.RequireRequestObject = input.RequireRequestObject;
            client.AllowRememberConsent = input.AllowRememberConsent;
            client.Enabled = input.Enabled;
            client.AllowOfflineAccess = input.AllowOfflineAccess;
            client.FrontChannelLogoutUri = input.FrontChannelLogoutUri;
            client.FrontChannelLogoutSessionRequired = input.FrontChannelLogoutSessionRequired;
            client.BackChannelLogoutUri = input.BackChannelLogoutUri;
            client.BackChannelLogoutSessionRequired = input.BackChannelLogoutSessionRequired;
            client.AllowedIdentityTokenSigningAlgorithms = input.AllowedIdentityTokenSigningAlgorithms;

            //token
            client.AccessTokenLifetime = input.AccessTokenLifetime;
            client.AccessTokenType = input.AccessTokenType;
            client.ConsentLifetime = input.ConsentLifetime;
            client.PairWiseSubjectSalt = input.PairWiseSubjectSalt;
            client.IncludeJwtId = input.IncludeJwtId;
            client.UserSsoLifetime = input.UserSsoLifetime;
            client.UserCodeType = input.UserCodeType;
            client.DeviceCodeLifetime = input.DeviceCodeLifetime;
            client.RequirePkce = input.RequirePkce;
            client.RequireClientSecret = input.RequireClientSecret;

            //AllowedCorsOrigins
            if (input.AllowedCorsOrigins != null)
            {
                client.RemoveAllCorsOrigins();
                foreach (var corsOrigin in input.AllowedCorsOrigins)
                {
                    client.AddCorsOrigin(corsOrigin);
                }
            }

            //AllowedGrantTypes
            if (input.AllowedGrantTypes != null)
            {
                client.RemoveAllAllowedGrantTypes();
                foreach (var allowedGrantType in input.AllowedGrantTypes)
                {
                    client.AddGrantType(allowedGrantType);
                }
            }

            //


            await ClientRepository.UpdateAsync(client);

        }

    }
}
