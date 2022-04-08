using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;

namespace SharpAbp.Abp.Identity
{
    [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
    public class IdentityUserAppService : IdentityAppServiceBase, IIdentityUserAppService
    {
        protected IdentityUserManager UserManager { get; }
        protected IIdentityUserRepository UserRepository { get; }
        protected IIdentityRoleRepository RoleRepository { get; }
        protected IOrganizationUnitRepository OrganizationUnitRepository { get; }
        protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }
        protected IOptions<IdentityOptions> IdentityOptions { get; }

        public IdentityUserAppService(
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository,
            IIdentityRoleRepository roleRepository,
            IOrganizationUnitRepository organizationUnitRepository,
            IIdentityClaimTypeRepository claimTypeRepository,
            IOptions<IdentityOptions> identityOptions)
        {
            UserManager = userManager;
            UserRepository = userRepository;
            RoleRepository = roleRepository;
            OrganizationUnitRepository = organizationUnitRepository;
            ClaimTypeRepository = claimTypeRepository;
            IdentityOptions = identityOptions;
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //TODO: [Authorize(IdentityPermissions.Users.Default)] should go the IdentityUserAppService class.
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<IdentityUserDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<IdentityUser, IdentityUserDto>(
                await UserManager.GetByIdAsync(id)
            );
        }

        /// <summary>
        /// Find by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<IdentityUserDto> FindByUsernameAsync(string userName)
        {
            return ObjectMapper.Map<IdentityUser, IdentityUserDto>(
                await UserManager.FindByNameAsync(userName)
            );
        }


        /// <summary>
        /// Find by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<IdentityUserDto> FindByEmailAsync(string email)
        {
            return ObjectMapper.Map<IdentityUser, IdentityUserDto>(
                await UserManager.FindByEmailAsync(email)
            );
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
        {
            var count = await UserRepository.GetCountAsync(input.Filter);
            var list = await UserRepository.GetListAsync(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.Filter);

            return new PagedResultDto<IdentityUserDto>(
                count,
                ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(list)
            );
        }

        /// <summary>
        /// Get all claimTypes
        /// </summary>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<List<IdentityClaimTypeDto>> GetAllClaimTypesAsync()
        {
            var identityClaimTypes = await ClaimTypeRepository.GetListAsync(false, default);
            return ObjectMapper.Map<List<IdentityClaimType>, List<IdentityClaimTypeDto>>(identityClaimTypes);
        }

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id)
        {
            //TODO: Should also include roles of the related OUs.

            var roles = await UserRepository.GetRolesAsync(id);

            return new ListResultDto<IdentityRoleDto>(
                ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(roles)
            );
        }

        /// <summary>
        /// Get assignable roles
        /// </summary>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
        {
            var list = await RoleRepository.GetListAsync();
            return new ListResultDto<IdentityRoleDto>(
                ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list));
        }

        /// <summary>
        /// Get organizationUnits
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<List<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id)
        {
            var organizationUnits = await UserRepository.GetOrganizationUnitsAsync(id);
            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits);
        }

        /// <summary>
        /// Get available organizationUnits
        /// </summary>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<ListResultDto<OrganizationUnitDto>> GetAvailableOrganizationUnitsAsync()
        {
            var organizationUnits = await OrganizationUnitRepository.GetListAsync(true, default);
            return new ListResultDto<OrganizationUnitDto>(
                ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits)
                );
        }

        /// <summary>
        /// Get claims
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<List<IdentityUserClaimDto>> GetClaimsAsync(Guid id)
        {
            var user = await UserRepository.GetAsync(id);
            return ObjectMapper.Map<List<IdentityUserClaim>, List<IdentityUserClaimDto>>(user.Claims.ToList());
        }


        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Create)]
        public virtual async Task<IdentityUserDto> CreateAsync(NewIdentityUserCreateDto input)
        {
            await IdentityOptions.SetAsync();

            var user = new IdentityUser(
                GuidGenerator.Create(),
                input.UserName,
                input.Email,
                CurrentTenant.Id
            );

            input.MapExtraPropertiesTo(user);

            (await UserManager.CreateAsync(user, input.Password)).CheckErrors();
            await UpdateUserByInput(user, input);

            //Update organizationUnits
            await UpdateUserOrganizationUnits(user, input.OrganizationUnitIds);

            (await UserManager.UpdateAsync(user)).CheckErrors();

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Update)]
        public virtual async Task<IdentityUserDto> UpdateAsync(Guid id, NewIdentityUserUpdateDto input)
        {
            await IdentityOptions.SetAsync();

            var user = await UserManager.GetByIdAsync(id);

            //4.2.2之后版本
            //user.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            (await UserManager.SetUserNameAsync(user, input.UserName)).CheckErrors();

            await UpdateUserByInput(user, input);

            //Update organizationUnits
            await UpdateUserOrganizationUnits(user, input.OrganizationUnitIds);

            input.MapExtraPropertiesTo(user);

            (await UserManager.UpdateAsync(user)).CheckErrors();

            if (!input.Password.IsNullOrEmpty())
            {
                (await UserManager.RemovePasswordAsync(user)).CheckErrors();
                (await UserManager.AddPasswordAsync(user, input.Password)).CheckErrors();
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            if (CurrentUser.Id == id)
            {
                throw new BusinessException(code: IdentityErrorCodes.UserSelfDeletion);
            }

            var user = await UserManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return;
            }

            (await UserManager.DeleteAsync(user)).CheckErrors();
        }

        /// <summary>
        /// Update roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Update)]
        public virtual async Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input)
        {
            var user = await UserManager.GetByIdAsync(id);
            (await UserManager.SetRolesAsync(user, input.RoleNames)).CheckErrors();
            await UserRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Update claims
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityUserClaims"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Update)]
        public virtual async Task UpdateClaimsAsync(Guid id, List<CreateOrUpdateIdentityUserClaimDto> identityUserClaims)
        {
            var user = await UserRepository.GetAsync(id);

            var claims = identityUserClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

            var removeClaims = user.Claims.Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .Except(claims, new ClaimEqualityComparer())
                .ToList();

            foreach (var addClaim in claims)
            {
                var claim = user.FindClaim(addClaim);
                if (claim == null)
                {
                    user.AddClaim(GuidGenerator, addClaim);
                }
            }

            user.RemoveClaims(removeClaims);

            await UserRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Lock user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Update)]
        public virtual async Task LockAsync(Guid id, int seconds)
        {
            var user = await UserRepository.GetAsync(id);
            await UserManager.SetLockoutEnabledAsync(user, true);
            await UserManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddSeconds(seconds));
        }

        /// <summary>
        /// Unlock user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Update)]
        public virtual async Task UnLockAsync(Guid id)
        {
            var user = await UserRepository.GetAsync(id);
            await UserManager.SetLockoutEnabledAsync(user, false);
        }

        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Update)]
        public virtual async Task SetPasswordAsync(Guid id, SetPasswordDto input)
        {
            var user = await UserRepository.GetAsync(id);
            if (!input.NewPassword.IsNullOrEmpty())
            {
                (await UserManager.RemovePasswordAsync(user)).CheckErrors();
                (await UserManager.AddPasswordAsync(user, input.NewPassword)).CheckErrors();
            }
        }

        /// <summary>
        /// two-factor enabled
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Default)]
        public virtual async Task<bool> TwoFactorEnableAsync(Guid id)
        {
            var user = await UserRepository.GetAsync(id);
            return user.TwoFactorEnabled;
        }

        /// <summary>
        /// Set two-factor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [Authorize(Volo.Abp.Identity.IdentityPermissions.Users.Update)]
        public virtual async Task SetTwoFactorAsync(Guid id, bool enabled)
        {
            var user = await UserRepository.GetAsync(id);
            await UserManager.SetTwoFactorEnabledAsync(user, enabled);
        }


        protected virtual async Task UpdateUserByInput(IdentityUser user, IdentityUserCreateOrUpdateDtoBase input)
        {
            if (!string.Equals(user.Email, input.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                (await UserManager.SetEmailAsync(user, input.Email)).CheckErrors();
            }

            if (!string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
            }

            (await UserManager.SetLockoutEnabledAsync(user, input.LockoutEnabled)).CheckErrors();

            user.Name = input.Name;
            user.Surname = input.Surname;
            (await UserManager.UpdateAsync(user)).CheckErrors();

            if (input.RoleNames != null)
            {
                (await UserManager.SetRolesAsync(user, input.RoleNames)).CheckErrors();
            }
        }

        protected virtual async Task UpdateUserOrganizationUnits(IdentityUser user, params Guid[] organizationUnitIds)
        {
            if (organizationUnitIds != null && organizationUnitIds.Any())
            {
                await UserManager.SetOrganizationUnitsAsync(user, organizationUnitIds);
            }
        }
    }
}
