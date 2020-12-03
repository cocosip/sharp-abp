using SharpAbp.Abp.FileStoring;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringAppService : FileStoringManagementAppServiceBase, IFileStoringAppService
    {
        protected AbpFileStoringOptions Options { get; }

        /// <summary>
        /// Get all providers
        /// </summary>
        /// <returns></returns>
        public Task<List<ProviderDto>> GetProvidersAsync()
        {
            var providers = ObjectMapper.Map<List<FileProviderConfiguration>, List<ProviderDto>>(Options.Providers.GetFileProviders());
            return Task.FromResult(providers);
        }



    }
}
