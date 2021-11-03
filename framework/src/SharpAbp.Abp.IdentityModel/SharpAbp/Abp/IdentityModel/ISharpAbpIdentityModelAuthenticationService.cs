using System.Threading.Tasks;

namespace SharpAbp.Abp.IdentityModel
{
    public interface ISharpAbpIdentityModelAuthenticationService
    {
        /// <summary>
        /// Get user accessToken
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <param name="identityClientName"></param>
        /// <returns></returns>
        Task<string> GetUserAccessTokenAsync(string userName, string userPassword, string identityClientName = null);

        /// <summary>
        /// ExternalCredentials
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <param name="identityClientName"></param>
        /// <returns></returns>
        Task<string> GetExternalCredentialsAccessTokenAsync(string loginProvider, string providerKey, string identityClientName = null);
    }
}
