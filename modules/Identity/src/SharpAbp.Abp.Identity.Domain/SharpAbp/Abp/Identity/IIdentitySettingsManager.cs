using System.Threading.Tasks;

namespace SharpAbp.Abp.Identity
{
    public interface IIdentitySettingsManager
    {
        /// <summary>
        /// Get lockout configuration
        /// </summary>
        /// <returns></returns>
        Task<IdentityLockoutSettings> GetLockoutSettingsAsync();

        /// <summary>
        /// Set lockout configuration
        /// </summary>
        /// <param name="identityLockout"></param>
        /// <returns></returns>
        Task SetLockoutSettingsAsync(IdentityLockoutSettings identityLockout);

        /// <summary>
        /// Get password configuration
        /// </summary>
        /// <returns></returns>
        Task<IdentityPasswordSettings> GetPasswordSettingsAsync();

        /// <summary>
        /// Set password configuration
        /// </summary>
        /// <param name="identityPassword"></param>
        /// <returns></returns>
        Task SetPasswordSettingsAsync(IdentityPasswordSettings identityPassword);

        /// <summary>
        /// Get user configuration
        /// </summary>
        /// <returns></returns>
        Task<IdentityUserSettings> GetUserSettingsAsync();

        /// <summary>
        /// Set user configuration
        /// </summary>
        /// <param name="identityUser"></param>
        /// <returns></returns>
        Task SetIdentityUserSettingsAsync(IdentityUserSettings identityUser);

        /// <summary>
        /// Get sign configuration
        /// </summary>
        /// <returns></returns>
        Task<IdentitySignInSettings> GetSignInSettingsAsync();

        /// <summary>
        /// Set signIn configuration
        /// </summary>
        /// <param name="identitySignIn"></param>
        /// <returns></returns>
        Task SetSignInSettingsAsync(IdentitySignInSettings identitySignIn);
    }
}
