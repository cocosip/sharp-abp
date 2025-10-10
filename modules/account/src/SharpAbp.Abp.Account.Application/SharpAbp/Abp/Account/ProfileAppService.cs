using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Settings;
using Volo.Abp.Users;

namespace SharpAbp.Abp.Account
{
    /// <summary>
    /// Application service implementation for managing user profile operations.
    /// Provides functionality for retrieving, updating user profiles and changing passwords.
    /// Requires user authorization for all operations.
    /// </summary>
    [Authorize]
    public class ProfileAppService : IdentityAppServiceBase, IProfileAppService
    {
        /// <summary>
        /// Gets the identity user manager for user operations.
        /// </summary>
        protected IdentityUserManager UserManager { get; }
        
        /// <summary>
        /// Gets the identity options configuration.
        /// </summary>
        protected IOptions<IdentityOptions> IdentityOptions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileAppService"/> class.
        /// </summary>
        /// <param name="userManager">The identity user manager for user operations.</param>
        /// <param name="identityOptions">The identity options configuration.</param>
        public ProfileAppService(
            IdentityUserManager userManager,
            IOptions<IdentityOptions> identityOptions)
        {
            UserManager = userManager;
            IdentityOptions = identityOptions;
        }

        /// <summary>
        /// Gets the current user's profile information.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user's profile data.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the current user is not found.</exception>
        public virtual async Task<ProfileDto> GetAsync()
        {
            var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

            return ObjectMapper.Map<IdentityUser, ProfileDto>(currentUser);
        }

        /// <summary>
        /// Updates the current user's profile information including username, email, phone number, name and surname.
        /// Validates user permissions for username and email updates based on system settings.
        /// </summary>
        /// <param name="input">The profile update data containing new user information and concurrency stamp.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated profile data.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the current user is not found.</exception>
        /// <exception cref="BusinessException">Thrown when update validation fails or concurrency conflicts occur.</exception>
        public virtual async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
        {
            await IdentityOptions.SetAsync();

            var user = await UserManager.GetByIdAsync(CurrentUser.GetId());

            user.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            if (!string.Equals(user.UserName, input.UserName, StringComparison.InvariantCultureIgnoreCase))
            {
                if (await SettingProvider.IsTrueAsync(IdentitySettingNames.User.IsUserNameUpdateEnabled))
                {
                    (await UserManager.SetUserNameAsync(user, input.UserName)).CheckErrors();
                }
            }

            if (!string.Equals(user.Email, input.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                if (await SettingProvider.IsTrueAsync(IdentitySettingNames.User.IsEmailUpdateEnabled))
                {
                    (await UserManager.SetEmailAsync(user, input.Email)).CheckErrors();
                }
            }

            if (!string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
            }

            user.Name = input.Name;
            user.Surname = input.Surname;

            input.MapExtraPropertiesTo(user);

            (await UserManager.UpdateAsync(user)).CheckErrors();

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<IdentityUser, ProfileDto>(user);
        }

        /// <summary>
        /// Changes the current user's password.
        /// For external users, password change is not allowed.
        /// If user has no existing password, adds a new password instead of changing.
        /// </summary>
        /// <param name="input">The password change data containing current password and new password.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the current user is not found.</exception>
        /// <exception cref="BusinessException">Thrown when the user is external or password validation fails.</exception>
        public virtual async Task ChangePasswordAsync(ChangePasswordInput input)
        {
            await IdentityOptions.SetAsync();

            var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

            if (currentUser.IsExternal)
            {
                throw new BusinessException(code: IdentityErrorCodes.ExternalUserPasswordChange);
            }

            if (currentUser.PasswordHash == null)
            {
                (await UserManager.AddPasswordAsync(currentUser, input.NewPassword)).CheckErrors();

                return;
            }

            (await UserManager.ChangePasswordAsync(currentUser, input.CurrentPassword, input.NewPassword)).CheckErrors();
        }
    }
}