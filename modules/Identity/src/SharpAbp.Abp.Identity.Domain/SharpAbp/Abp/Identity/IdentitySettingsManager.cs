using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity.Settings;
using Volo.Abp.SettingManagement;

namespace SharpAbp.Abp.Identity
{
    /// <summary>
    /// Implementation of identity settings manager that provides functionality for managing identity-related settings.
    /// This class handles the retrieval and persistence of lockout, password, user, and sign-in configurations using the ABP setting management system.
    /// </summary>
    public class IdentitySettingsManager : IIdentitySettingsManager, ITransientDependency
    {
        /// <summary>
        /// Gets or sets the setting manager used for persisting and retrieving settings from the underlying storage.
        /// </summary>
        protected ISettingManager SettingManager { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentitySettingsManager"/> class.
        /// </summary>
        /// <param name="settingManager">The setting manager instance used for managing application settings.</param>
        public IdentitySettingsManager(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        /// <summary>
        /// Retrieves the current lockout settings configuration for user accounts.
        /// This method fetches lockout-related settings including whether lockout is allowed for new users,
        /// the duration of lockout periods, and the maximum number of failed access attempts before lockout occurs.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current lockout settings with all configuration values.</returns>
        public virtual async Task<IdentityLockoutSettings> GetLockoutSettingsAsync()
        {
            var allowedForNewUsersValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Lockout.AllowedForNewUsers);
            bool.TryParse(allowedForNewUsersValue, out bool allowedForNewUsers);

            var lockoutDurationValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Lockout.LockoutDuration);
            int.TryParse(lockoutDurationValue, out int lockoutDuration);

            var maxFailedAccessAttemptsValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Lockout.MaxFailedAccessAttempts);
            int.TryParse(maxFailedAccessAttemptsValue, out int maxFailedAccessAttempts);

            var identityLockout = new IdentityLockoutSettings()
            {
                AllowedForNewUsers = allowedForNewUsers,
                LockoutDuration = lockoutDuration,
                MaxFailedAccessAttempts = maxFailedAccessAttempts
            };
            return identityLockout;
        }

        /// <summary>
        /// Updates the lockout settings configuration for user accounts.
        /// This method persists the provided lockout settings to the global configuration store,
        /// affecting all users in the system regarding lockout behavior and thresholds.
        /// </summary>
        /// <param name="identityLockout">The lockout settings object containing the new configuration values including lockout duration, maximum failed attempts, and enablement status.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task SetLockoutSettingsAsync(IdentityLockoutSettings identityLockout)
        {
            await SettingManager.SetGlobalAsync(IdentitySettingNames.Lockout.AllowedForNewUsers, identityLockout.AllowedForNewUsers.ToString());

            await SettingManager.SetGlobalAsync(IdentitySettingNames.Lockout.LockoutDuration, identityLockout.LockoutDuration.ToString());

            await SettingManager.SetGlobalAsync(IdentitySettingNames.Lockout.MaxFailedAccessAttempts, identityLockout.MaxFailedAccessAttempts.ToString());
        }

        /// <summary>
        /// Retrieves the current password policy settings for user accounts.
        /// This method fetches all password-related requirements including character type requirements,
        /// minimum length constraints, and unique character requirements that govern password creation and updates.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current password policy settings with all validation rules.</returns>
        public virtual async Task<IdentityPasswordSettings> GetPasswordSettingsAsync()
        {
            var requireDigitValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Password.RequireDigit);
            bool.TryParse(requireDigitValue, out bool requireDigit);

            var requireLowercaseValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Password.RequireLowercase);
            bool.TryParse(requireLowercaseValue, out bool requireLowercase);

            var requireNonAlphanumericValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Password.RequireNonAlphanumeric);
            bool.TryParse(requireNonAlphanumericValue, out bool requireNonAlphanumeric);

            var requireUppercaseValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Password.RequireUppercase);
            bool.TryParse(requireUppercaseValue, out bool requireUppercase);

            var requiredLengthValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Password.RequiredLength);
            int.TryParse(requiredLengthValue, out int requiredLength);

            var requiredUniqueCharsValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.Password.RequiredUniqueChars);
            int.TryParse(requiredUniqueCharsValue, out int requiredUniqueChars);

            var identityPassword = new IdentityPasswordSettings()
            {
                RequireDigit = requireDigit,
                RequireLowercase = requireLowercase,
                RequireNonAlphanumeric = requireNonAlphanumeric,
                RequireUppercase = requireUppercase,
                RequiredLength = requiredLength,
                RequiredUniqueChars = requiredUniqueChars
            };
            return identityPassword;
        }

        /// <summary>
        /// Updates the password policy settings for user accounts.
        /// This method persists the provided password policy settings to the global configuration store,
        /// establishing new password complexity requirements that will be enforced for all users.
        /// </summary>
        /// <param name="identityPassword">The password settings object containing the new policy configuration including character requirements, length constraints, and uniqueness rules.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task SetPasswordSettingsAsync(IdentityPasswordSettings identityPassword)
        {
            await SettingManager.SetGlobalAsync(IdentitySettingNames.Password.RequireDigit, identityPassword.RequireDigit.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.Password.RequireLowercase, identityPassword.RequireLowercase.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.Password.RequireNonAlphanumeric, identityPassword.RequireNonAlphanumeric.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.Password.RequireUppercase, identityPassword.RequireUppercase.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.Password.RequiredLength, identityPassword.RequiredLength.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.Password.RequiredUniqueChars, identityPassword.RequiredUniqueChars.ToString());
        }

        /// <summary>
        /// Retrieves the current user account settings configuration.
        /// This method fetches user-related settings that control user account management capabilities,
        /// including permissions for email and username modifications.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current user account settings with permission configurations.</returns>
        public virtual async Task<IdentityUserSettings> GetUserSettingsAsync()
        {
            var isEmailUpdateEnabledValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.User.IsEmailUpdateEnabled);
            bool.TryParse(isEmailUpdateEnabledValue, out bool isEmailUpdateEnabled);

            var isUserNameUpdateEnabledValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.User.IsUserNameUpdateEnabled);
            bool.TryParse(isUserNameUpdateEnabledValue, out bool isUserNameUpdateEnabled);

            var identityUser = new IdentityUserSettings()
            {
                IsEmailUpdateEnabled = isEmailUpdateEnabled,
                IsUserNameUpdateEnabled = isUserNameUpdateEnabled,
            };
            return identityUser;
        }

        /// <summary>
        /// Updates the user account settings configuration.
        /// This method persists the provided user settings to the global configuration store,
        /// controlling user account management permissions and capabilities across the system.
        /// </summary>
        /// <param name="identityUser">The user settings object containing the new configuration values for user account management permissions.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task SetIdentityUserSettingsAsync(IdentityUserSettings identityUser)
        {
            await SettingManager.SetGlobalAsync(IdentitySettingNames.User.IsEmailUpdateEnabled, identityUser.IsEmailUpdateEnabled.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.User.IsUserNameUpdateEnabled, identityUser.IsUserNameUpdateEnabled.ToString());
        }

        /// <summary>
        /// Retrieves the current sign-in settings configuration for user authentication.
        /// This method fetches authentication-related settings that control sign-in requirements,
        /// including email confirmation, phone number confirmation, and related authentication policies.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current sign-in settings with all authentication requirements.</returns>
        public virtual async Task<IdentitySignInSettings> GetSignInSettingsAsync()
        {
            var enablePhoneNumberConfirmationValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation);
            bool.TryParse(enablePhoneNumberConfirmationValue, out bool enablePhoneNumberConfirmation);

            var requireConfirmedEmailValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail);
            bool.TryParse(requireConfirmedEmailValue, out bool requireConfirmedEmail);

            var requireConfirmedPhoneNumberValue = await SettingManager.GetOrNullGlobalAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber);
            bool.TryParse(requireConfirmedPhoneNumberValue, out bool requireConfirmedPhoneNumber);

            var identitySignIn = new IdentitySignInSettings()
            {
                EnablePhoneNumberConfirmation = enablePhoneNumberConfirmation,
                RequireConfirmedEmail = requireConfirmedEmail,
                RequireConfirmedPhoneNumber = requireConfirmedPhoneNumber
            };
            return identitySignIn;
        }

        /// <summary>
        /// Updates the sign-in settings configuration for user authentication.
        /// This method persists the provided sign-in settings to the global configuration store,
        /// establishing new authentication requirements and policies for user sign-in processes.
        /// </summary>
        /// <param name="identitySignIn">The sign-in settings object containing the new configuration values for authentication requirements and confirmation policies.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task SetSignInSettingsAsync(IdentitySignInSettings identitySignIn)
        {
            await SettingManager.SetGlobalAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail, identitySignIn.RequireConfirmedEmail.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber, identitySignIn.RequireConfirmedPhoneNumber.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation, identitySignIn.EnablePhoneNumberConfirmation.ToString());
        }

    }
}
