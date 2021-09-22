using JetBrains.Annotations;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.IdentityServer
{
    public interface IIdentityServerClientManager : IDomainService
    {
        /// <summary>
        /// Validate clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task ValidateClientIdAsync([NotNull] string clientId);
    }
}
