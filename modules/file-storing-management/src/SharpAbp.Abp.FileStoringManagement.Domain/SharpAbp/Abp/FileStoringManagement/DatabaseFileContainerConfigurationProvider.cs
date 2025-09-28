using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using System;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Provides file container configurations from database sources.
    /// This implementation of <see cref="IFileContainerConfigurationProvider"/> retrieves
    /// container configurations from the database via caching mechanisms for improved performance.
    /// It replaces the default configuration provider to enable dynamic container management.
    /// </summary>
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IFileContainerConfigurationProvider))]
    public class DatabaseFileContainerConfigurationProvider : IFileContainerConfigurationProvider
    {
        /// <summary>
        /// Gets the configuration converter for transforming cache items to configurations.
        /// This converter handles the transformation between cached container data and
        /// runtime configuration objects used by the file storage system.
        /// </summary>
        protected IFileContainerConfigurationConverter ConfigurationConverter { get; }
        
        /// <summary>
        /// Gets the container cache manager for retrieving cached container data.
        /// This manager provides high-performance access to container information
        /// through distributed caching with automatic fallback to database queries.
        /// </summary>
        protected IFileStoringContainerCacheManager ContainerCacheManager { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFileContainerConfigurationProvider"/> class.
        /// </summary>
        /// <param name="configurationConverter">The configuration converter for data transformation.</param>
        /// <param name="containerCacheManager">The cache manager for container data access.</param>
        public DatabaseFileContainerConfigurationProvider(
            IFileContainerConfigurationConverter configurationConverter,
            IFileStoringContainerCacheManager containerCacheManager)
        {
            ConfigurationConverter = configurationConverter;
            ContainerCacheManager = containerCacheManager;
        }


        /// <summary>
        /// Retrieves the file container configuration for the specified container name.
        /// This method provides synchronous access to container configurations by internally
        /// calling the asynchronous implementation using <see cref="AsyncHelper.RunSync"/>.
        /// The configuration is retrieved from cache if available, otherwise loaded from database.
        /// </summary>
        /// <param name="name">The unique name of the container. Cannot be null or whitespace.</param>
        /// <returns>A <see cref="FileContainerConfiguration"/> object containing the container's runtime configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the name parameter is null or whitespace.</exception>
        /// <exception cref="AbpException">Thrown when no container configuration is found for the specified name.</exception>
        public virtual FileContainerConfiguration Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return AsyncHelper.RunSync(() =>
            {
                return GetConfigurationAsync(name);
            });
        }

        /// <summary>
        /// Asynchronously retrieves the file container configuration for the specified container name.
        /// This method first attempts to retrieve the container from cache for optimal performance.
        /// If the container is found in cache, it converts the cached data to a runtime configuration.
        /// If no container is found, it throws an exception indicating the configuration is missing.
        /// </summary>
        /// <param name="name">The unique name of the container to retrieve configuration for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a <see cref="FileContainerConfiguration"/> object with the container's runtime configuration.
        /// </returns>
        /// <exception cref="AbpException">Thrown when no container configuration is found for the specified name.</exception>
        protected virtual async Task<FileContainerConfiguration> GetConfigurationAsync(string name)
        {
            var cacheItem = await ContainerCacheManager.GetAsync(name);
            if (cacheItem != null)
            {
                return ConfigurationConverter.ToConfiguration(cacheItem);
            }
            throw new AbpException($"Could not find FileContainerConfiguration by name '{name}'.");
        }

    }
}
