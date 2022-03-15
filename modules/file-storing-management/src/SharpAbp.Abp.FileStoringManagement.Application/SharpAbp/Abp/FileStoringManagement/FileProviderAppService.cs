using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public virtual Task<List<ProviderDto>> GetProvidersAsync()
        {
            var providerConfigurations = Options.Providers.GetFileProviders();
            var providers = ObjectMapper.Map<List<FileProviderConfiguration>, List<ProviderDto>>(providerConfigurations);
            return Task.FromResult(providers);
        }

        /// <summary>
        /// Contain provider or not
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Providers.Default)]
        public virtual Task<bool> HasProviderAsync([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));
            var configuration = Options.Providers.GetConfiguration(provider);
            return Task.FromResult(configuration is not null);
        }

        /// <summary>
        /// Get provider options
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Providers.Options)]
        public virtual Task<ProviderOptionsDto> GetOptionsAsync([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            var fileProviderConfiguration = Options.Providers.GetConfiguration(provider);
            if (fileProviderConfiguration == null)
            {
                throw new UserFriendlyException($"Could not get provider configuration by name '{provider}'.");
            }

            var values = fileProviderConfiguration.GetValueTypes();
            var providerOptions = new ProviderOptionsDto(provider);

            var ll = StringLocalizerFactory.Create(fileProviderConfiguration.LocalizationResource);

            foreach (var kv in values)
            {
                var providerValue = new ProviderValueDto(
                    kv.Key, ll[kv.Key],
                    TypeHelper.GetFullNameHandlingNullableAndGenerics(kv.Value));

                providerOptions.Values.Add(providerValue);
            }

            return Task.FromResult(providerOptions);
        }
    }
}
