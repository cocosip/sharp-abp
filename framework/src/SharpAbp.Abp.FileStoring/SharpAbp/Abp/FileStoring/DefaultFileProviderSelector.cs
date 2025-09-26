﻿using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Default implementation of <see cref="IFileProviderSelector"/> that selects file providers based on container configurations.
    /// </summary>
    public class DefaultFileProviderSelector : IFileProviderSelector, ITransientDependency
    {
        /// <summary>
        /// The service provider used to resolve file provider services.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// The configuration provider used to retrieve container configurations.
        /// </summary>
        protected IFileContainerConfigurationProvider ConfigurationProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileProviderSelector"/> class.
        /// </summary>
        /// <param name="configurationProvider">The configuration provider used to retrieve container configurations.</param>
        /// <param name="serviceProvider">The service provider used to resolve file provider services.</param>
        public DefaultFileProviderSelector(
            IFileContainerConfigurationProvider configurationProvider,
            IServiceProvider serviceProvider)
        {
            ConfigurationProvider = configurationProvider;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the file provider for the specified container name.
        /// </summary>
        /// <param name="containerName">The name of the container for which to retrieve the file provider.</param>
        /// <returns>The file provider associated with the specified container name.</returns>
        /// <exception cref="Volo.Abp.AbpException">
        /// Thrown when no container configuration is found for the specified container name.
        /// </exception>
        [NotNull]
        public virtual IFileProvider Get([NotNull] string containerName)
        {
            Check.NotNull(containerName, nameof(containerName));
            
            var configuration = ConfigurationProvider.Get(containerName);
            if (configuration == null)
            {
                throw new AbpException($"No container configuration found for container name '{containerName}'. Please ensure the container is properly configured.");
            }

            return ServiceProvider.GetRequiredKeyedService<IFileProvider>(configuration.Provider);
        }
    }
}