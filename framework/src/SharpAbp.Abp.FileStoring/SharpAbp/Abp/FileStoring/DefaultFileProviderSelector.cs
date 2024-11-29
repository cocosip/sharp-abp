using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFileProviderSelector : IFileProviderSelector, ITransientDependency
    {
        protected IServiceProvider ServiceProvider { get; }
        protected IFileContainerConfigurationProvider ConfigurationProvider { get; }

        public DefaultFileProviderSelector(
            IFileContainerConfigurationProvider configurationProvider,
            IServiceProvider serviceProvider)
        {
            ConfigurationProvider = configurationProvider;
            ServiceProvider = serviceProvider;
        }

        [NotNull]
        public virtual IFileProvider Get([NotNull] string containerName)
        {
            Check.NotNull(containerName, nameof(containerName));
            var configuration = ConfigurationProvider.Get(containerName);
            if (configuration == null)
            {
                throw new AbpException($"Could not find container configuration by name '{containerName}'.");
            }

            return ServiceProvider.GetRequiredKeyedService<IFileProvider>(configuration.Provider);
        }
    }
}