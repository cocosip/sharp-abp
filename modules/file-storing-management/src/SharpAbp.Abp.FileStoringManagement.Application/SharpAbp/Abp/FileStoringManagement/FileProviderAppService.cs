﻿using JetBrains.Annotations;
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
    /// <summary>
    /// Application service for managing file storing providers.
    /// Provides functionality to discover available providers and retrieve their configuration options.
    /// </summary>
    [Authorize(FileStoringManagementPermissions.Providers.Default)]
    public class FileProviderAppService : FileStoringManagementAppServiceBase, IFileProviderAppService
    {
        /// <summary>
        /// Gets the file storing abstractions options configuration.
        /// </summary>
        protected AbpFileStoringAbstractionsOptions Options { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderAppService"/> class.
        /// </summary>
        /// <param name="options">The file storing abstractions options.</param>
        public FileProviderAppService(IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Retrieves all available file storing providers in the system.
        /// </summary>
        /// <returns>A list of provider information containing provider names and configurations.</returns>
        [Authorize(FileStoringManagementPermissions.Providers.Default)]
        public virtual Task<List<ProviderDto>> GetProvidersAsync()
        {
            var providerConfigurations = Options.Providers.GetFileProviders();
            var providers = ObjectMapper.Map<List<FileProviderConfiguration>, List<ProviderDto>>(providerConfigurations);
            return Task.FromResult(providers);
        }

        /// <summary>
        /// Checks whether a specific file storing provider exists in the system.
        /// </summary>
        /// <param name="provider">The name of the provider to check.</param>
        /// <returns>True if the provider exists; otherwise, false.</returns>
        [Authorize(FileStoringManagementPermissions.Providers.Default)]
        public virtual Task<bool> HasProviderAsync([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));
            var configuration = Options.Providers.GetConfiguration(provider);
            return Task.FromResult(configuration is not null);
        }

        /// <summary>
        /// Retrieves the configuration options available for a specific file storing provider.
        /// </summary>
        /// <param name="provider">The name of the provider to get options for.</param>
        /// <returns>The provider options including all available configuration parameters with their types, examples, and localized descriptions.</returns>
        /// <exception cref="UserFriendlyException">Thrown when the specified provider configuration cannot be found.</exception>
        [Authorize(FileStoringManagementPermissions.Providers.Options)]
        public virtual Task<ProviderOptionsDto> GetOptionsAsync([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            var fileProviderConfiguration = Options.Providers.GetConfiguration(provider) ?? throw new UserFriendlyException($"Could not find provider configuration for '{provider}'. Please ensure the provider is properly registered.");
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
