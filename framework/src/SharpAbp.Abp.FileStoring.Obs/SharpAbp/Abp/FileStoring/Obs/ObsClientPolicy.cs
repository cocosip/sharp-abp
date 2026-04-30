using OBS;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsClientPolicy : IObjectPoolPolicy<ObsClient>
    {
        protected IObsClientFactory ObsClientFactory { get; }
        protected ObsFileProviderConfiguration ObsFileProviderConfiguration { get; }

        public ObsClientPolicy(
            IObsClientFactory obsClientFactory,
            ObsFileProviderConfiguration obsFileProviderConfiguration)
        {
            ObsClientFactory = obsClientFactory;
            ObsFileProviderConfiguration = obsFileProviderConfiguration;
        }

        public ObsClient Create()
        {
            return ObsClientFactory.Create(ObsFileProviderConfiguration);
        }

        public bool Return(ObsClient obj)
        {
            return obj != null;
        }
    }
}
