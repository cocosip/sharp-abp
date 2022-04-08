using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [Authorize(IdentityPermissions.IdentityClaimTypes.Default)]
    public class IdentityClaimTypeAppService : IdentityAppServiceBase, IIdentityClaimTypeAppService
    {
        protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }
        protected IdentityClaimTypeManager ClaimTypeManager { get; }
        public IdentityClaimTypeAppService(
            IIdentityClaimTypeRepository claimTypeRepository,
            IdentityClaimTypeManager claimTypeManager)
        {
            ClaimTypeRepository = claimTypeRepository;
            ClaimTypeManager = claimTypeManager;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentityClaimTypes.Default)]
        public virtual async Task<IdentityClaimTypeDto> GetAsync(Guid id)
        {
            var identityClaimType = await ClaimTypeRepository.GetAsync(id);
            return ObjectMapper.Map<IdentityClaimType, IdentityClaimTypeDto>(identityClaimType);
        }

        /// <summary>
        /// Any
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoredId"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentityClaimTypes.Default)]
        public virtual async Task<bool> AnyAsync(string name, Guid? ignoredId = null)
        {
            return await ClaimTypeRepository.AnyAsync(name, ignoredId);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentityClaimTypes.Default)]
        public virtual async Task<PagedResultDto<IdentityClaimTypeDto>> GetPagedListAsync(IdentityClaimTypePagedRequestDto input)
        {
            var count = await ClaimTypeRepository.GetCountAsync(input.Filter);
            var identityClaimTypes = await ClaimTypeRepository.GetListAsync(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.Filter);

            return new PagedResultDto<IdentityClaimTypeDto>(
              count,
              ObjectMapper.Map<List<IdentityClaimType>, List<IdentityClaimTypeDto>>(identityClaimTypes)
              );
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentityClaimTypes.Create)]
        public virtual async Task<Guid> CreateAsync(CreateIdentityClaimTypeDto input)
        {
            var identityClaimType = new IdentityClaimType(
                GuidGenerator.Create(),
                input.Name,
                input.Required,
                input.IsStatic,
                input.Regex,
                input.RegexDescription,
                input.Description,
                input.ValueType);

            await ClaimTypeManager.CreateAsync(identityClaimType);
            return identityClaimType.Id;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentityClaimTypes.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateIdentityClaimTypeDto input)
        {
            var identityClaimType = await ClaimTypeRepository.GetAsync(id);
            identityClaimType.Required = input.Required;
            identityClaimType.Regex = input.Regex;
            identityClaimType.RegexDescription = input.RegexDescription;
            identityClaimType.Description = input.Description;
            await ClaimTypeManager.UpdateAsync(identityClaimType);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentityClaimTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await ClaimTypeRepository.DeleteAsync(id);
        }

    }
}
