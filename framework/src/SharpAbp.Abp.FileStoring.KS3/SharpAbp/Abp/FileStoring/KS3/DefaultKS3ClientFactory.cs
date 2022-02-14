using KS3;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public class DefaultKS3ClientFactory : IKS3ClientFactory, ITransientDependency
    {
        public IKS3 Create(KS3FileProviderConfiguration configuration)
        {
            Check.NotNullOrWhiteSpace(configuration.AccessKey, nameof(configuration.AccessKey));
            Check.NotNullOrWhiteSpace(configuration.SecretKey, nameof(configuration.SecretKey));
            Check.NotNullOrWhiteSpace(configuration.Endpoint, nameof(configuration.Endpoint));

            return new KS3Client(configuration.AccessKey, configuration.SecretKey, new ClientConfiguration()
            {
                Protocol = configuration.Protocol,
                UserAgent = configuration.UserAgent,
                MaxConnections = configuration.MaxConnections,
                Timeout = configuration.Timeout,
                ReadWriteTimeout = configuration.ReadWriteTimeout
            });
        }
    }
}
