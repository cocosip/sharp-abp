﻿using Microsoft.AspNetCore.Authorization;
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
                //var value = secret.Type == "SharedSecret" ? secret.Value : IdentityServer4.Models.HashExtensions.Sha256(secret.Value);
                var value = IdentityServer4.Models.HashExtensions.Sha256(secret.Value);
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

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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

            //Claims
            var claims = input.Claims.Select(x => new System.Security.Claims.Claim(x.Type, x.Value)).ToList();

            var removeClaims = client.Claims.Select(x => new System.Security.Claims.Claim(x.Type, x.Value))
                .Except(claims)
                .ToList();

            foreach (var claim in claims)
            {
                var clientClaim = client.FindClaim(claim.Value, claim.Type);
                if (clientClaim == null)
                {
                    client.AddClaim(claim.Value, claim.Type);
                }
            }

            foreach (var removeClaim in removeClaims)
            {
                client.RemoveClaim(removeClaim.Value, removeClaim.Type);
            }

            //ClientSecrets
            var inputClientSecrets = input.ClientSecrets
                .Select(x => new CreateOrUpdateClientSecretDto()
                {
                    Value = x.ClientId.HasValue ? x.Value : IdentityServer4.Models.HashExtensions.Sha256(x.Value),
                    Type = x.Type,
                    Expiration = x.Expiration,
                    Description = x.Description
                })
                .ToList();

            var removeSecrets = client.ClientSecrets.Select(x => (x.Value, x.Type))
                .Except(inputClientSecrets.Select(y => (y.Value, y.Type)))
                .ToList();

            foreach (var clientSecret in inputClientSecrets)
            {
                var secret = client.FindSecret(clientSecret.Value, clientSecret.Type);
                if (secret == null)
                {
                    client.AddSecret(clientSecret.Value, clientSecret.Expiration, clientSecret.Type, clientSecret.Description);
                }
            }

            foreach (var removeSecret in removeSecrets)
            {
                client.RemoveSecret(removeSecret.Value, removeSecret.Type);
            }

            //IdentityProviderRestrictions
            if (input.IdentityProviderRestrictions != null)
            {
                client.RemoveAllIdentityProviderRestrictions();
                foreach (var identityProviderRestriction in input.IdentityProviderRestrictions)
                {
                    client.AddIdentityProviderRestriction(identityProviderRestriction);
                }
            }

            //PostLogoutRedirectUris
            if (input.PostLogoutRedirectUris != null)
            {
                client.RemoveAllPostLogoutRedirectUris();
                foreach (var postLogoutRedirectUri in input.PostLogoutRedirectUris)
                {
                    client.AddPostLogoutRedirectUri(postLogoutRedirectUri);
                }
            }

            //Properties
            var removeProperties = client.Properties.Select(x => (x.Key, x.Value))
                .Except(input.Properties.Select(y => (y.Key, y.Value)))
                .ToList();

            foreach (var property in input.Properties)
            {
                var clientProperty = client.FindProperty(property.Key);
                if (clientProperty == null)
                {
                    client.AddProperty(property.Key, property.Value);
                }
            }

            foreach (var removeProperty in removeProperties)
            {
                client.RemoveProperty(removeProperty.Key);
            }

            //RedirectUris
            if (input.RedirectUris != null)
            {
                client.RemoveAllRedirectUris();
                foreach (var redirectUri in input.RedirectUris)
                {
                    client.AddRedirectUri(redirectUri);
                }
            }

            //Scopes
            if (input.Scopes != null)
            {
                client.RemoveAllScopes();
                foreach (var scope in input.Scopes)
                {
                    client.AddScope(scope);
                }
            }

            await ClientRepository.UpdateAsync(client);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.Clients.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await ClientRepository.DeleteAsync(id);
        }

    }
}
