using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Default)]
    public class IdentityRoleAppService : IdentityAppServiceBase, IIdentityRoleAppService
    {
        protected IIdentityRoleRepository IdentityRoleRepository { get; }
        protected IIdentityClaimTypeRepository IdentityClaimTypeRepository { get; }
        protected IdentityRoleManager IdentityRoleManager { get; }
        public IdentityRoleAppService(
            IIdentityRoleRepository identityRoleRepository,
            IIdentityClaimTypeRepository identityClaimTypeRepository,
            IdentityRoleManager identityRoleManager)
        {
            IdentityRoleRepository = identityRoleRepository;
            IdentityClaimTypeRepository = identityClaimTypeRepository;
            IdentityRoleManager = identityRoleManager;
        }

        /// <summary>
        /// Get all claimTypes
        /// </summary>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Roles.Default)]
        public virtual async Task<List<IdentityClaimTypeDto>> GetAllClaimTypes()
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
            var identityRole = await IdentityRoleRepository.GetAsync(id);
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
            var identityRole = await IdentityRoleRepository.GetAsync(id);

            var claims = roleClaims.Select(x => new Claim(x.ClaimType, x.Value)).ToList();

            var removeClaims = identityRole.Claims.Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .Except(claims)
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

            await IdentityRoleRepository.UpdateAsync(identityRole);
        }

    }
}
