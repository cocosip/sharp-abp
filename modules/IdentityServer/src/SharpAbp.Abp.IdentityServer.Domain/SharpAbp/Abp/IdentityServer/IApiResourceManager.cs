using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.IdentityServer.ApiResources;

namespace SharpAbp.Abp.IdentityServer
{
    public interface IApiResourceManager : IDomainService
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="apiResource"></param>
        /// <returns></returns>
        Task CreateAsync(ApiResource apiResource);
    }
}
