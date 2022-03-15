using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileProviderAppService : IApplicationService
    {
        /// <summary>
        /// Get providers
        /// </summary>
        /// <returns></returns>
        Task<List<ProviderDto>> GetProvidersAsync();

        /// <summary>
        /// Contain provider or not
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        Task<bool> HasProviderAsync([NotNull] string provider);

        /// <summary>
        /// Get provider options
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        Task<ProviderOptionsDto> GetOptionsAsync([NotNull] string provider);
    }
}
