using JetBrains.Annotations;
using System.Collections.Generic;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileProviderAppService : IApplicationService
    {
        /// <summary>
        /// Get providers
        /// </summary>
        /// <returns></returns>
        List<ProviderDto> GetProviders();

        /// <summary>
        /// Contain provider or not
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        bool HasProvider([NotNull] string provider);

        /// <summary>
        /// Get provider options
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        ProviderOptionsDto GetOptions([NotNull] string provider);
    }
}
