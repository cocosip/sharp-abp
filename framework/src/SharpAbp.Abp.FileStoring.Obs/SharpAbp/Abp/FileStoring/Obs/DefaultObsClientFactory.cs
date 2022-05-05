using OBS;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class DefaultObsClientFactory : IObsClientFactory, ITransientDependency
    {

        public virtual ObsClient Create(ObsFileProviderConfiguration configuration)
        {
            Check.NotNullOrWhiteSpace(configuration.AccessKeyId, nameof(configuration.AccessKeyId));
            Check.NotNullOrWhiteSpace(configuration.AccessKeySecret, nameof(configuration.AccessKeySecret));
            Check.NotNullOrWhiteSpace(configuration.Endpoint, nameof(configuration.Endpoint));
            return new ObsClient(configuration.Endpoint, configuration.AccessKeyId, configuration.AccessKeySecret);
        }

    }
}
