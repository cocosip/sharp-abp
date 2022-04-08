using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;

namespace SharpAbp.Abp.Identity
{
    [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Default)]
    public class IdentityRoleAppService : IdentityAppServiceBase, IIdentityRoleAppService
    {
        protected IdentityRoleManager RoleManager { get; }
        protected IIdentityRoleRepository RoleRepository { get; }
        protected IIdentityClaimTypeRepository IdentityClaimTypeRepository { get; }

        public IdentityRoleAppService(
            IdentityRoleManager roleManager,
            IIdentityRoleRepository roleRepository,
            IIdentityClaimTypeRepository identityClaimTypeRepository)
        {
            RoleManager = roleManager;
            RoleRepository = roleRepository;
            IdentityClaimTypeRepository = identityClaimTypeRepository;
        }

        /// <summary>
        /// Get all claimTypes
        /// </summary>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Default)]
        public virtual async Task<List<IdentityClaimTypeDto>> GetAllClaimTypesAsync()
        {
            var identityClaimTypes = await IdentityClaimTypeRepository.GetListAsync(false, default);
            return ObjectMapper.Map<List<IdentityClaimType>, List<IdentityClaimTypeDto>>(identityClaimTypes);
        }

        /// <summary>
        /// Get role claims
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Default)]
        public virtual async Task<List<IdentityRoleClaimDto>> GetClaimsAsync(Guid id)
        {
            var identityRole = await RoleRepository.GetAsync(id);
            return ObjectMapper.Map<List<IdentityRoleClaim>, List<IdentityRoleClaimDto>>(identityRole.Claims.ToList());
        }

        /// <summary>
        /// Create or Update claims
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleClaims"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Update)]
        public virtual async Task CreateOrUpdateClaimsAsync(Guid id, List<CreateOrUpdateIdentityRoleClaimDto> roleClaims)
        {
            var identityRole = await RoleRepository.GetAsync(id);

            var claims = roleClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

            var removeClaims = identityRole.Claims.Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .Except(claims, new ClaimEqualityComparer())
                .ToList();

            foreach (var addClaim in claims)
            {
                var claim = identityRole.FindClaim(addClaim);
                if (claim == null)
                {
                    identityRole.AddClaim(GuidGenerator, addClaim);
                }
            }

            foreach (var claim in removeClaims)
            {
                identityRole.RemoveClaim(claim);
            }

            await RoleRepository.UpdateAsync(identityRole);
        }

        public virtual async Task<IdentityRoleDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<IdentityRole, IdentityRoleDto>(
                await RoleManager.GetByIdAsync(id)
            );
        }

        public virtual async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
        {
            var list = await RoleRepository.GetListAsync();
            return new ListResultDto<IdentityRoleDto>(
                ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list)
            );
        }

        public virtual async Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRolesInput input)
        {
            var list = await RoleRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter);
            var totalCount = await RoleRepository.GetCountAsync(input.Filter);

            return new PagedResultDto<IdentityRoleDto>(
                totalCount,
                ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list)
                );
        }

        [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Create)]
        public virtual async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
        {
            var role = new IdentityRole(
                GuidGenerator.Create(),
                input.Name,
                CurrentTenant.Id
            )
            {
                IsDefault = input.IsDefault,
                IsPublic = input.IsPublic
            };

            input.MapExtraPropertiesTo(role);

            (await RoleManager.CreateAsync(role)).CheckErrors();
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<IdentityRole, IdentityRoleDto>(role);
        }

        [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Update)]
        public virtual async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
        {
            var role = await RoleManager.GetByIdAsync(id);

            role.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            (await RoleManager.SetRoleNameAsync(role, input.Name)).CheckErrors();

            role.IsDefault = input.IsDefault;
            role.IsPublic = input.IsPublic;

            input.MapExtraPropertiesTo(role);

            (await RoleManager.UpdateAsync(role)).CheckErrors();
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<IdentityRole, IdentityRoleDto>(role);
        }

        [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            var role = await RoleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return;
            }

            (await RoleManager.DeleteAsync(role)).CheckErrors();
        }

    }
}
