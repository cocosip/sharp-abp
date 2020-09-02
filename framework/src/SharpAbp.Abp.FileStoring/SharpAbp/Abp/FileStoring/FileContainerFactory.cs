using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace Volo.Abp.FileStoring
{
    public class FileContainerFactory : IFileContainerFactory, ITransientDependency
    {
        protected IFileProviderSelector ProviderSelector { get; }

        protected IFileContainerConfigurationProvider ConfigurationProvider { get; }

        protected ICurrentTenant CurrentTenant { get; }

        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        protected IServiceProvider ServiceProvider { get; }

        public FileContainerFactory(
            IFileContainerConfigurationProvider configurationProvider,
            ICurrentTenant currentTenant,
            ICancellationTokenProvider cancellationTokenProvider,
            IFileProviderSelector providerSelector,
            IServiceProvider serviceProvider)
        {
            ConfigurationProvider = configurationProvider;
            CurrentTenant = currentTenant;
            CancellationTokenProvider = cancellationTokenProvider;
            ProviderSelector = providerSelector;
            ServiceProvider = serviceProvider;
        }

        public virtual IFileContainer Create(string name)
        {
            var configuration = ConfigurationProvider.Get(name);

            return new FileContainer(
                name,
                configuration,
                ProviderSelector.Get(name),
                CurrentTenant,
                CancellationTokenProvider,
                ServiceProvider
            );
        }
    }
}
