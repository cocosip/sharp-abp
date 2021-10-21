using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.IdentityResources;

namespace SharpAbp.Abp.IdentityServer.IdentityResources
{
    [Authorize(IdentityServerPermissions.IdentityResources.Default)]
    public class IdentityResourceAppService : IdentityServerAppServiceBase, IIdentityResourceAppService
    {
        protected IIdentityResourceRepository IdentityResourceRepository { get; }
        public IdentityResourceAppService(IIdentityResourceRepository identityResourceRepository)
        {
            IdentityResourceRepository = identityResourceRepository;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.IdentityResources.Default)]
        public virtual async Task<IdentityResourceDto> GetAsync(Guid id)
        {
            var identityResource = await IdentityResourceRepository.GetAsync(id);
            return ObjectMapper.Map<IdentityResource, IdentityResourceDto>(identityResource);
        }

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.IdentityResources.Default)]
        public virtual async Task<IdentityResourceDto> FindByNameAsync(string name)
        {
            var identityResource = await IdentityResourceRepository.FindByNameAsync(name);
            return ObjectMapper.Map<IdentityResource, IdentityResourceDto>(identityResource);
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.IdentityResources.Default)]
        public virtual async Task<List<IdentityResourceDto>> GetAllAsync()
        {
            var identityResources = await IdentityResourceRepository.GetListAsync(false, default);
            return ObjectMapper.Map<List<IdentityResource>, List<IdentityResourceDto>>(identityResources);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.IdentityResources.Default)]
        public virtual async Task<PagedResultDto<IdentityResourceDto>> GetPagedListAsync(IdentityResourcePagedRequestDto input)
        {
            var count = await IdentityResourceRepository.GetCountAsync(input.Filter);
            var identityResources = await IdentityResourceRepository.GetListAsync(
                input.Sorting,
                input.SkipCount,
                input.MaxResultCount,
                input.Filter);

            return new PagedResultDto<IdentityResourceDto>(
              count,
              ObjectMapper.Map<List<IdentityResource>, List<IdentityResourceDto>>(identityResources)
              );
        }

        /// <summary>
        /// Get list by scope name
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.IdentityResources.Default)]
        public virtual async Task<List<IdentityResourceDto>> GetListByScopeNameAsync(string[] scopeNames)
        {
            if (scopeNames == null)
            {
                scopeNames = new string[] { };
            }
            var identityResources = await IdentityResourceRepository.GetListByScopeNameAsync(scopeNames);
            return ObjectMapper.Map<List<IdentityResource>, List<IdentityResourceDto>>(identityResources);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.IdentityResources.Create)]
        public virtual async Task<Guid> CreateAsync(CreateIdentityResourceDto input)
        {
            await CheckNameExistAsync(input.Name);

            var identityResource = new IdentityResource(
                GuidGenerator.Create(),
                input.Name,
                input.DisplayName,
                input.Description,
                input.Enabled,
                input.Required,
                input.Emphasize,
                input.ShowInDiscoveryDocument);

            foreach (var createClaim in input.UserClaims)
            {
                identityResource.AddUserClaim(createClaim.Type);
            }

            await IdentityResourceRepository.InsertAsync(identityResource);

            return identityResource.Id;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.IdentityResources.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateIdentityResourceDto input)
        {
            await CheckNameExistAsync(input.Name, id);
            var identityResource = await IdentityResourceRepository.GetAsync(id);
            identityResource.Name = input.Name;
            identityResource.DisplayName = input.DisplayName;
            identityResource.Description = input.Description;
            identityResource.Enabled = input.Enabled;
            identityResource.Required = input.Required;
            identityResource.Emphasize = input.Emphasize;
            identityResource.ShowInDiscoveryDocument = input.ShowInDiscoveryDocument;

            //claim
            var removeClaimTypes = identityResource.UserClaims.Select(x => x.Type)
                .Except(input.UserClaims.Select(y => y.Type))
                .ToList();

            foreach (var createClaim in input.UserClaims)
            {
                var claim = identityResource.FindUserClaim(createClaim.Type);
                if (claim == null)
                {
                    identityResource.AddUserClaim(createClaim.Type);
                }
            }

            foreach (var claimType in removeClaimTypes)
            {
                identityResource.RemoveUserClaim(claimType);
            }

            //property
            var removePropertyKeys = identityResource.Properties.Select(x => x.Key)
                .Except(input.Properties.Select(y => y.Key))
                .ToList();

            foreach (var createProperty in input.Properties)
            {
                var property = identityResource.FindProperty(createProperty.Key);
                if (property == null)
                {
                    identityResource.AddProperty(createProperty.Key, createProperty.Value);
                }
            }

            foreach (var propertyKey in removePropertyKeys)
            {
                identityResource.RemoveProperty(propertyKey);
            }

            await IdentityResourceRepository.UpdateAsync(identityResource);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.IdentityResources.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await IdentityResourceRepository.DeleteAsync(id);
        }

        protected virtual async Task CheckNameExistAsync(string name, Guid? expectedId = null)
        {
            if (await IdentityResourceRepository.CheckNameExistAsync(name, expectedId))
            {
                throw new UserFriendlyException(L["IdentityServer.DumplicateIdentityResourceName", name]);
            }
        }

    }
}
