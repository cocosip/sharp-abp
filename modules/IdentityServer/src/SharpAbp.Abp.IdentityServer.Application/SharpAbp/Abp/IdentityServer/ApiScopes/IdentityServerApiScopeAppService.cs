using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.ApiScopes;

namespace SharpAbp.Abp.IdentityServer.ApiScopes
{
    [Authorize(IdentityServerPermissions.ApiScopes.Default)]
    public class IdentityServerApiScopeAppService : IdentityServerAppServiceBase, IIdentityServerApiScopeAppService
    {
        protected IApiScopeRepository ApiScopeRepository { get; }
        public IdentityServerApiScopeAppService(IApiScopeRepository apiScopeRepository)
        {
            ApiScopeRepository = apiScopeRepository;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiScopes.Default)]
        public virtual async Task<ApiScopeDto> GetAsync(Guid id)
        {
            var apiScope = await ApiScopeRepository.GetAsync(id);
            return ObjectMapper.Map<ApiScope, ApiScopeDto>(apiScope);
        }

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiScopes.Default)]
        public virtual async Task<ApiScopeDto> FindByName(string name)
        {
            var apiScope = await ApiScopeRepository.GetByNameAsync(name);
            return ObjectMapper.Map<ApiScope, ApiScopeDto>(apiScope);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiScopes.Default)]
        public virtual async Task<PagedResultDto<ApiScopeDto>> GetPagedListAsync(ApiScopePagedRequestDto input)
        {
            var count = await ApiScopeRepository.GetCountAsync(input.Filter);
            var apiScopes = await ApiScopeRepository.GetListAsync(
                input.Sorting,
                input.SkipCount,
                input.MaxResultCount,
                input.Filter);

            return new PagedResultDto<ApiScopeDto>(
              count,
              ObjectMapper.Map<List<ApiScope>, List<ApiScopeDto>>(apiScopes)
              );
        }

        /// <summary>
        /// Get list by scope names
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiScopes.Default)]
        public virtual async Task<List<ApiScopeDto>> GetListByNameAsync(string[] scopeNames)
        {
            if (scopeNames == null)
            {
                scopeNames = new string[] { };
            }

            var apiScopes = await ApiScopeRepository.GetListByNameAsync(scopeNames);
            return ObjectMapper.Map<List<ApiScope>, List<ApiScopeDto>>(apiScopes);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiScopes.Create)]
        public virtual async Task<Guid> CreateAsync(CreateApiScopeDto input)
        {
            await CheckNameExistAsync(input.Name);
            var apiScope = new ApiScope(
                GuidGenerator.Create(),
                input.Name,
                input.DisplayName,
                input.Description,
                input.Required,
                input.Emphasize,
                input.ShowInDiscoveryDocument,
                input.Enabled);

            foreach (var createClaim in input.UserClaims)
            {
                apiScope.AddUserClaim(createClaim.Type);
            }

            await ApiScopeRepository.InsertAsync(apiScope);
            return apiScope.Id;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiScopes.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateApiScopeDto input)
        {
            await CheckNameExistAsync(input.Name, id);

            var apiScope = await ApiScopeRepository.GetAsync(id);
            apiScope.DisplayName = input.DisplayName;
            apiScope.Description = input.Description;
            apiScope.Enabled = input.Enabled;
            apiScope.Required = input.Required;
            apiScope.Emphasize = input.Emphasize;
            apiScope.ShowInDiscoveryDocument = input.ShowInDiscoveryDocument;

            //claim
            var removeClaimTypes = apiScope.UserClaims.Select(x => x.Type)
                .Except(input.UserClaims.Select(y => y.Type))
                .ToList();

            foreach (var createClaim in input.UserClaims)
            {
                var claim = apiScope.FindClaim(createClaim.Type);
                if (claim == null)
                {
                    apiScope.AddUserClaim(createClaim.Type);
                }
            }

            foreach (var claimType in removeClaimTypes)
            {
                apiScope.RemoveClaim(claimType);
            }

            //property
            var removePropertyKeys = apiScope.Properties.Select(x => x.Key)
                .Except(input.Properties.Select(y => y.Key))
                .ToList();

            foreach (var createProperty in input.Properties)
            {
                var property = apiScope.FindProperty(createProperty.Key);
                if (property == null)
                {
                    apiScope.AddProperty(createProperty.Key, createProperty.Value);
                }
            }

            foreach (var propertyKey in removePropertyKeys)
            {
                apiScope.RemoveProperty(propertyKey);
            }

            await ApiScopeRepository.UpdateAsync(apiScope);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiScopes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await ApiScopeRepository.DeleteAsync(id);
        }

        protected virtual async Task CheckNameExistAsync(string name, Guid? expectedId = null)
        {
            if (await ApiScopeRepository.CheckNameExistAsync(name, expectedId))
            {
                throw new UserFriendlyException(L["IdentityServer.DumplicateApiScopeName", name]);
            }
        }

    }
}
