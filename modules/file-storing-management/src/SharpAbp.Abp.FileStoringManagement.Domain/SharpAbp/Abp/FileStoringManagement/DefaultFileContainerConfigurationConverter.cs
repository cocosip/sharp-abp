﻿using System;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Provides the default implementation for converting file storing container entities
    /// to file container configurations. This converter transforms database entities
    /// and cache items into runtime configuration objects used by the file storage system.
    /// </summary>
    public class DefaultFileContainerConfigurationConverter : IFileContainerConfigurationConverter, ITransientDependency
    {
        /// <summary>
        /// Gets the file storing abstractions options that contain provider configurations.
        /// These options define the available providers and their configuration schemas.
        /// </summary>
        protected AbpFileStoringAbstractionsOptions Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileContainerConfigurationConverter"/> class.
        /// </summary>
        /// <param name="options">The options containing file storing provider configurations.</param>
        public DefaultFileContainerConfigurationConverter(IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Converts a database entity 'FileStoringContainer' to a 'FileContainerConfiguration'.
        /// This method transforms the persistent container entity into a runtime configuration
        /// object that can be used by the file storage system. It applies type conversions
        /// for configuration values and sets up default naming normalizers from the provider.
        /// </summary>
        /// <param name="container">The file storing container entity to convert.</param>
        /// <returns>A file container configuration object ready for use by the file storage system.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provider configuration is not found.</exception>
        public virtual FileContainerConfiguration ToConfiguration(FileStoringContainer container)
        {
            var fileProviderConfiguration = Options.Providers.GetConfiguration(container.Provider);
            Check.NotNull(fileProviderConfiguration, nameof(fileProviderConfiguration));

            var configuration = new FileContainerConfiguration()
            {
                Provider = fileProviderConfiguration.Provider,
                IsMultiTenant = container.IsMultiTenant,
                EnableAutoMultiPartUpload = container.EnableAutoMultiPartUpload,
                MultiPartUploadMinFileSize = container.MultiPartUploadMinFileSize,
                MultiPartUploadShardingSize = container.MultiPartUploadShardingSize,
                HttpAccess = container.HttpAccess
            };

            foreach (var item in container.Items)
            {
                var providerItem = fileProviderConfiguration.GetItem(item.Name);
                var value = TypeHelper.ConvertFromString(providerItem.ValueType, item.Value);
                configuration.SetConfiguration(item.Name, value);
            }

            foreach (var namingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
            {
                configuration.NamingNormalizers.Add(namingNormalizer);
            }

            return configuration;
        }

        /// <summary>
        /// Converts a 'FileStoringContainerCacheItem' to a 'FileContainerConfiguration'.
        /// This method transforms cached container data into a runtime configuration object
        /// that can be used by the file storage system. It applies type conversions for
        /// configuration values and sets up default naming normalizers from the provider.
        /// This overload is optimized for cache scenarios to reduce database queries.
        /// </summary>
        /// <param name="cacheItem">The cached file storing container item to convert.</param>
        /// <returns>A file container configuration object ready for use by the file storage system.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provider configuration is not found.</exception>
        public virtual FileContainerConfiguration ToConfiguration(FileStoringContainerCacheItem cacheItem)
        {
            var fileProviderConfiguration = Options.Providers.GetConfiguration(cacheItem.Provider);
            Check.NotNull(fileProviderConfiguration, nameof(fileProviderConfiguration));

            var configuration = new FileContainerConfiguration()
            {
                Provider = fileProviderConfiguration.Provider,
                IsMultiTenant = cacheItem.IsMultiTenant,
                EnableAutoMultiPartUpload = cacheItem.EnableAutoMultiPartUpload,
                MultiPartUploadMinFileSize = cacheItem.MultiPartUploadMinFileSize,
                MultiPartUploadShardingSize = cacheItem.MultiPartUploadShardingSize,
                HttpAccess = cacheItem.HttpAccess
            };

            foreach (var item in cacheItem.Items)
            {
                var providerItem = fileProviderConfiguration.GetItem(item.Name);

                var value = TypeHelper.ConvertFromString(providerItem.ValueType, item.Value);
                configuration.SetConfiguration(item.Name, value);
            }

            foreach (var namingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
            {
                configuration.NamingNormalizers.Add(namingNormalizer);
            }

            return configuration;
        }
    }
}
