using System.Threading.Tasks;

namespace SharpAbp.Abp.Identity
{
    /// <summary>
    /// Provides methods for managing identity-related settings including lockout, password, user, and sign-in configurations.
    /// This interface defines the contract for retrieving and updating various identity settings in the system.
    /// </summary>
    public interface IIdentitySettingsManager
    {
        /// <summary>
        /// Retrieves the current lockout settings configuration for user accounts.
        /// This includes settings for lockout duration, maximum failed access attempts, and whether lockout is enabled for new users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current lockout settings.</returns>
        Task<IdentityLockoutSettings> GetLockoutSettingsAsync();

        /// <summary>
        /// Updates the lockout settings configuration for user accounts.
        /// This method allows modification of lockout duration, maximum failed access attempts, and lockout enablement for new users.
        /// </summary>
        /// <param name="identityLockout">The lockout settings object containing the new configuration values to be applied.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetLockoutSettingsAsync(IdentityLockoutSettings identityLockout);

        /// <summary>
        /// Retrieves the current password policy settings for user accounts.
        /// This includes requirements for digits, lowercase letters, uppercase letters, non-alphanumeric characters, minimum length, and unique characters.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current password settings.</returns>
        Task<IdentityPasswordSettings> GetPasswordSettingsAsync();

        /// <summary>
        /// Updates the password policy settings for user accounts.
        /// This method allows modification of password complexity requirements including character types, length, and uniqueness constraints.
        /// </summary>
        /// <param name="identityPassword">The password settings object containing the new policy configuration values to be applied.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetPasswordSettingsAsync(IdentityPasswordSettings identityPassword);

        /// <summary>
        /// Retrieves the current user account settings configuration.
        /// This includes settings for email update permissions and username update permissions.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current user settings.</returns>
        Task<IdentityUserSettings> GetUserSettingsAsync();

        /// <summary>
        /// Updates the user account settings configuration.
        /// This method allows modification of user account permissions such as email and username update capabilities.
        /// </summary>
        /// <param name="identityUser">The user settings object containing the new configuration values to be applied.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetIdentityUserSettingsAsync(IdentityUserSettings identityUser);

        /// <summary>
        /// Retrieves the current sign-in settings configuration for user authentication.
        /// This includes settings for email confirmation requirements, phone number confirmation, and related sign-in policies.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current sign-in settings.</returns>
        Task<IdentitySignInSettings> GetSignInSettingsAsync();

        /// <summary>
        /// Updates the sign-in settings configuration for user authentication.
        /// This method allows modification of sign-in requirements including email and phone number confirmation policies.
        /// </summary>
        /// <param name="identitySignIn">The sign-in settings object containing the new configuration values to be applied.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetSignInSettingsAsync(IdentitySignInSettings identitySignIn);
    }
}
