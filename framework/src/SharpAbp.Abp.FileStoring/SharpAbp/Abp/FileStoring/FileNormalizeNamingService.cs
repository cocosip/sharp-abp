﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Service implementation for normalizing file naming conventions.
    /// This service applies configured naming normalizers to container names and file names
    /// to ensure consistency across different file storage providers.
    /// </summary>
    public class FileNormalizeNamingService : IFileNormalizeNamingService, ITransientDependency
    {
        /// <summary>
        /// The service provider used to resolve naming normalizer instances.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Initializes a new instance of the FileNormalizeNamingService class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to resolve dependencies</param>
        public FileNormalizeNamingService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Normalizes both container name and file name based on the provided configuration.
        /// Applies all configured naming normalizers to the names.
        /// </summary>
        /// <param name="configuration">The file container configuration containing naming normalizers</param>
        /// <param name="containerName">The original container name to normalize</param>
        /// <param name="fileName">The original file name to normalize</param>
        /// <returns>A FileNormalizeNaming object containing the normalized container and file names</returns>
        public virtual FileNormalizeNaming NormalizeNaming(
            FileContainerConfiguration configuration,
            string? containerName,
            string? fileName)
        {
            // If no naming normalizers are configured, return the original names
            if (!configuration.NamingNormalizers.Any())
            {
                return new FileNormalizeNaming(containerName, fileName);
            }

            // Create a service scope to resolve normalizer instances
            using var scope = ServiceProvider.CreateScope();
            
            // Apply each configured naming normalizer
            foreach (var normalizerType in configuration.NamingNormalizers)
            {
                var normalizer = scope.ServiceProvider
                    .GetRequiredService(normalizerType)
                    .As<IFileNamingNormalizer>();

                // Normalize container name if it's not null or whitespace
                containerName = containerName.IsNullOrWhiteSpace() ? containerName : normalizer.NormalizeContainerName(containerName);
                
                // Normalize file name if it's not null or whitespace
                fileName = fileName.IsNullOrWhiteSpace() ? fileName : normalizer.NormalizeFileName(fileName);
            }

            return new FileNormalizeNaming(containerName, fileName);
        }

        /// <summary>
        /// Normalizes a container name based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The file container configuration containing naming normalizers</param>
        /// <param name="containerName">The original container name to normalize</param>
        /// <returns>The normalized container name</returns>
        public string NormalizeContainerName(FileContainerConfiguration configuration, string containerName)
        {
            // If no naming normalizers are configured, return the original name
            if (!configuration.NamingNormalizers.Any())
            {
                return containerName;
            }

            // Use the combined normalization method with null file name
            return NormalizeNaming(configuration, containerName, null).ContainerName!;
        }

        /// <summary>
        /// Normalizes a file name based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The file container configuration containing naming normalizers</param>
        /// <param name="fileName">The original file name to normalize</param>
        /// <returns>The normalized file name</returns>
        public string NormalizeFileName(FileContainerConfiguration configuration, string fileName)
        {
            // If no naming normalizers are configured, return the original name
            if (!configuration.NamingNormalizers.Any())
            {
                return fileName;
            }

            // Use the combined normalization method with null container name
            return NormalizeNaming(configuration, null, fileName).FileName!;
        }
    }
}