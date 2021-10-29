using System.Threading.Tasks;

namespace SharpAbp.Abp.IdentityModel
{
    public interface IIdentityModelUserAuthenticationService
    {
        /// <summary>
        /// Get user accessToken
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <param name="identityClientName"></param>
        /// <returns></returns>
        Task<string> GetUserAccessTokenAsync(string userName, string userPassword, string identityClientName = null);
    }
}
