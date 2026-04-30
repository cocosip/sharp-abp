using OBS;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsClientPolicy : IObjectPoolPolicy<ObsClient>
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected ObsFileProviderConfiguration ObsFileProviderConfiguration { get; }

        public ObsClientPolicy(
            IServiceScopeFactory serviceScopeFactory,
            ObsFileProviderConfiguration obsFileProviderConfiguration)
        {
            ServiceScopeFactory = serviceScopeFactory;
            ObsFileProviderConfiguration = obsFileProviderConfiguration;
        }

        public ObsClient Create()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var obsClientFactory = scope.ServiceProvider.GetRequiredService<IObsClientFactory>();
            return obsClientFactory.Create(ObsFileProviderConfiguration);
        }

        public bool Return(ObsClient obj)
        {
            return obj != null;
        }
    }
}
