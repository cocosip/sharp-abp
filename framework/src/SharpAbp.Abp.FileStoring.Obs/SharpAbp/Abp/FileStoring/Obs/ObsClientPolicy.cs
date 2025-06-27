using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using OBS;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsClientPolicy : IPooledObjectPolicy<ObsClient>
    {
        protected IServiceProvider ServiceProvider { get; }
        protected ObsFileProviderConfiguration ObsFileProviderConfiguration { get; }
        public ObsClientPolicy(
            IServiceProvider serviceProvider,
            ObsFileProviderConfiguration obsFileProviderConfiguration)
        {
            ServiceProvider = serviceProvider;
            ObsFileProviderConfiguration = obsFileProviderConfiguration;
        }

        public ObsClient Create()
        {
            using var scope = ServiceProvider.CreateScope();
            var obsClientFactory = scope.ServiceProvider.GetRequiredService<IObsClientFactory>();
            return obsClientFactory.Create(ObsFileProviderConfiguration);
        }

        public bool Return(ObsClient obj)
        {
            return obj != null;
        }
    }
}
