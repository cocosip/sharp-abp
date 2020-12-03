using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileStoringAppService : IApplicationService
    {
        /// <summary>
        /// Get all providers
        /// </summary>
        /// <returns></returns>
        Task<List<ProviderDto>> GetProvidersAsync();
    }
}
