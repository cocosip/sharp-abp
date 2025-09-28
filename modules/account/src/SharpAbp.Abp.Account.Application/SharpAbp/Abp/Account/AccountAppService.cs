﻿﻿﻿﻿﻿﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Account.Emailing;
using SharpAbp.Abp.Account.Localization;
using SharpAbp.Abp.Account.Settings;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Settings;

namespace SharpAbp.Abp.Account
{
    /// <summary>
    /// Application service implementation for account management operations.
    /// Handles user registration, password reset, and related account functions.
    /// </summary>
    public class AccountAppService : ApplicationService, IAccountAppService
    {
        /// <summary>
        /// Gets the identity role repository for role management operations.
        /// </summary>
        protected IIdentityRoleRepository RoleRepository { get; }
        
        /// <summary>
        /// Gets the identity user manager for user operations.
        /// </summary>
        protected IdentityUserManager UserManager { get; }
        
        /// <summary>
        /// Gets the account emailer service for sending account-related emails.
        /// </summary>
        protected IAccountEmailer AccountEmailer { get; }
        
        /// <summary>
        /// Gets the identity security log manager for security audit logging.
        /// </summary>
        protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
        
        /// <summary>
        /// Gets the identity options configuration.
        /// </summary>
        protected IOptions<IdentityOptions> IdentityOptions { get; }

        /// <summary>
        /// Initializes a new instance of the AccountAppService class.
        /// </summary>
        /// <param name="userManager">The identity user manager.</param>
        /// <param name="roleRepository">The identity role repository.</param>
        /// <param name="accountEmailer">The account emailer service.</param>
        /// <param name="identitySecurityLogManager">The identity security log manager.</param>
        /// <param name="identityOptions">The identity options configuration.</param>
        public AccountAppService(
            IdentityUserManager userManager,
            IIdentityRoleRepository roleRepository,
            IAccountEmailer accountEmailer,
            IdentitySecurityLogManager identitySecurityLogManager,
            IOptions<IdentityOptions> identityOptions)
        {
            RoleRepository = roleRepository;
            AccountEmailer = accountEmailer;
            IdentitySecurityLogManager = identitySecurityLogManager;
            UserManager = userManager;
            IdentityOptions = identityOptions;

            LocalizationResource = typeof(AccountResource);
        }

        /// <summary>
        /// Registers a new user account in the system.
        /// Validates self-registration settings and creates a new user with the provided information.
        /// </summary>
        /// <param name="input">The registration input containing username, email, password, and additional properties.</param>
        /// <returns>The created user information as IdentityUserDto.</returns>
        /// <exception cref="UserFriendlyException">Thrown when self-registration is disabled.</exception>
        public virtual async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
        {
            await CheckSelfRegistrationAsync();

            await IdentityOptions.SetAsync();

            var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.EmailAddress, CurrentTenant.Id);

            input.MapExtraPropertiesTo(user);

            (await UserManager.CreateAsync(user, input.Password)).CheckErrors();

            await UserManager.SetEmailAsync(user, input.EmailAddress);
            await UserManager.AddDefaultRolesAsync(user);

            return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
        }

        /// <summary>
        /// Sends a password reset code to the specified email address.
        /// Generates a reset token and sends it via email with the appropriate return URL.
        /// </summary>
        /// <param name="input">The input containing email address and optional app name, return URL, and URL hash.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyException">Thrown when the email address is not found.</exception>
        public virtual async Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
        {
            var user = await GetUserByEmailAsync(input.Email);
            var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
            await AccountEmailer.SendPasswordResetLinkAsync(user, resetToken, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
        }

        /// <summary>
        /// Resets a user's password using the provided reset token.
        /// Validates the reset token and updates the user's password, then logs the security event.
        /// </summary>
        /// <param name="input">The input containing user ID, reset token, and new password.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="BusinessException">Thrown when the reset token is invalid or the user is not found.</exception>
        public virtual async Task ResetPasswordAsync(ResetPasswordDto input)
        {
            await IdentityOptions.SetAsync();

            var user = await UserManager.GetByIdAsync(input.UserId);
            (await UserManager.ResetPasswordAsync(user, input.ResetToken, input.Password)).CheckErrors();

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.ChangePassword
            });
        }

        /// <summary>
        /// Gets a user by their email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The user with the specified email address.</returns>
        /// <exception cref="UserFriendlyException">Thrown when no user is found with the specified email address.</exception>
        protected virtual async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UserFriendlyException(L["Volo.Account:InvalidEmailAddress", email]);
            }

            return user;
        }

        /// <summary>
        /// Checks if self-registration is enabled in the system settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyException">Thrown when self-registration is disabled.</exception>
        protected virtual async Task CheckSelfRegistrationAsync()
        {
            if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled))
            {
                throw new UserFriendlyException(L["SelfRegistrationDisabledMessage"]);
            }
        }
    }
}
