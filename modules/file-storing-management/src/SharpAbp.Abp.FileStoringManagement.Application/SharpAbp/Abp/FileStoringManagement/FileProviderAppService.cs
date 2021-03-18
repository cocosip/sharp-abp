using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.FileStoringManagement
{
    [Authorize(FileStoringManagementPermissions.Providers.Default)]
    public class FileProviderAppService : FileStoringManagementAppServiceBase, IFileProviderAppService
    {
        protected AbpFileStoringOptions Options { get; }
        public FileProviderAppService(IOptions<AbpFileStoringOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Get providers
        /// </summary>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Providers.Default)]
        public virtual List<ProviderDto> GetProviders()
        {
            var providerConfigurations = Options.Providers.GetFileProviders();
            return ObjectMapper.Map<List<FileProviderConfiguration>, List<ProviderDto>>(providerConfigurations);
        }

        /// <summary>
        /// Contain provider or not
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Providers.Default)]
        public virtual bool HasProvider([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));
            var configuration = Options.Providers.GetConfiguration(provider);
            return configuration != null;
        }

        /// <summary>
        /// Get provider options
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Providers.Options)]
        public virtual ProviderOptionsDto GetOptions([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            var fileProviderConfiguration = Options.Providers.GetConfiguration(provider);
            var values = fileProviderConfiguration.GetValues();
            var providerOptions = new ProviderOptionsDto(provider);

            foreach (var kv in values)
            {
                var providerValue = new ProviderValueDto(
                    kv.Key, L[kv.Key],
                    TypeHelper.GetFullNameHandlingNullableAndGenerics(kv.Value.Type),
                    kv.Value.Note);

                providerOptions.Values.Add(providerValue);
            }

            return providerOptions;
        }
    }
}
