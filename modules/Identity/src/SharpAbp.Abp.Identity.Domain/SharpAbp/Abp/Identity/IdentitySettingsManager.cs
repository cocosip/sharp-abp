using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity.Settings;
using Volo.Abp.SettingManagement;

namespace SharpAbp.Abp.Identity
{
    public class IdentitySettingsManager : IIdentitySettingsManager, ITransientDependency
    {
        protected ISettingManager SettingManager { get; set; }
        public IdentitySettingsManager(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        /// <summary>
        /// Get lockout configuration
        /// </summary>
        /// <returns></returns>
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
        /// Set lockout configuration
        /// </summary>
        /// <param name="identityLockout"></param>
        /// <returns></returns>
        public virtual async Task SetLockoutSettingsAsync(IdentityLockoutSettings identityLockout)
        {
            await SettingManager.SetGlobalAsync(IdentitySettingNames.Lockout.AllowedForNewUsers, identityLockout.AllowedForNewUsers.ToString());

            await SettingManager.SetGlobalAsync(IdentitySettingNames.Lockout.LockoutDuration, identityLockout.LockoutDuration.ToString());

            await SettingManager.SetGlobalAsync(IdentitySettingNames.Lockout.MaxFailedAccessAttempts, identityLockout.MaxFailedAccessAttempts.ToString());
        }

        /// <summary>
        /// Get password configuration
        /// </summary>
        /// <returns></returns>
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
        /// Set password configuration
        /// </summary>
        /// <param name="identityPassword"></param>
        /// <returns></returns>
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
        /// Get user configuration
        /// </summary>
        /// <returns></returns>
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
        /// Set user configuration
        /// </summary>
        /// <param name="identityUser"></param>
        /// <returns></returns>
        public virtual async Task SetIdentityUserSettingsAsync(IdentityUserSettings identityUser)
        {
            await SettingManager.SetGlobalAsync(IdentitySettingNames.User.IsEmailUpdateEnabled, identityUser.IsEmailUpdateEnabled.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.User.IsUserNameUpdateEnabled, identityUser.IsUserNameUpdateEnabled.ToString());
        }

        /// <summary>
        /// Get sign configuration
        /// </summary>
        /// <returns></returns>
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
        /// Set signIn configuration
        /// </summary>
        /// <param name="identitySignIn"></param>
        /// <returns></returns>
        public virtual async Task SetSignInSettingsAsync(IdentitySignInSettings identitySignIn)
        {
            await SettingManager.SetGlobalAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail, identitySignIn.RequireConfirmedEmail.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber, identitySignIn.RequireConfirmedPhoneNumber.ToString());
            await SettingManager.SetGlobalAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation, identitySignIn.EnablePhoneNumberConfirmation.ToString());
        }

    }
}
