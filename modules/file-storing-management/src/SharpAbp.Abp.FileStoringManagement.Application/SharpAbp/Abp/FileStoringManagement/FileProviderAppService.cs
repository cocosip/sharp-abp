using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.FileStoringManagement
{
    [Authorize(FileStoringManagementPermissions.Providers.Default)]
    public class FileProviderAppService : FileStoringManagementAppServiceBase, IFileProviderAppService
    {
        protected AbpFileStoringAbstractionsOptions Options { get; }
        public FileProviderAppService(IOptions<AbpFileStoringAbstractionsOptions> options)
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

            var providerItems = fileProviderConfiguration.GetItems();
            var providerOptions = new ProviderOptionsDto(provider);

            var ll = StringLocalizerFactory.Create(fileProviderConfiguration.LocalizationResource);

            foreach (var itemKeyValuePair in providerItems)
            {
                var providerItem = itemKeyValuePair.Value;
                var note = "";
                if (!providerItem.NoteLocalizationName.IsNullOrWhiteSpace())
                {
                    note = ll[providerItem.NoteLocalizationName];
                }
                var providerValue = new ProviderValueDto(
                    itemKeyValuePair.Key,
                    ll[itemKeyValuePair.Key],
                    TypeHelper.GetFullNameHandlingNullableAndGenerics(providerItem.ValueType),
                    providerItem.Eg,
                    note);

                providerOptions.Values.Add(providerValue);
            }
            
            return Task.FromResult(providerOptions);
        }
    }
}
