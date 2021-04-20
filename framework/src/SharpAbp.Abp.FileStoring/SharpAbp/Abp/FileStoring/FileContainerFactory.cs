using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerFactory : IFileContainerFactory, ITransientDependency
    {
        protected IFileProviderSelector ProviderSelector { get; }
        protected IFileContainerConfigurationProvider ConfigurationProvider { get; }
        protected IServiceProvider ServiceProvider { get; }

        public FileContainerFactory(
            IFileContainerConfigurationProvider configurationProvider,
            IFileProviderSelector providerSelector,
            IServiceProvider serviceProvider)
        {
            ConfigurationProvider = configurationProvider;
            ProviderSelector = providerSelector;
            ServiceProvider = serviceProvider;
        }

        public virtual IFileContainer Create(string name)
        {
            var configuration = ConfigurationProvider.Get(name);
            var fileProvider = ProviderSelector.Get(name);

            var fileContainer = ActivatorUtilities.CreateInstance<FileContainer>(
                ServiceProvider,
                name,
                configuration,
                fileProvider);

            return fileContainer;

            //return new FileContainer(
            //    name,
            //    configuration,
            //    ProviderSelector.Get(name),
            //    CurrentTenant,
            //    CancellationTokenProvider,
            //    FileNormalizeNamingService,
            //    ServiceProvider
            //);
        }
    }
}
