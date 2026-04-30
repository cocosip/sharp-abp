using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class S3ClientPolicy : IObjectPoolPolicy<IAmazonS3>
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected S3FileProviderConfiguration S3FileProviderConfiguration { get; }

        public S3ClientPolicy(
            IServiceScopeFactory serviceScopeFactory,
            S3FileProviderConfiguration s3FileProviderConfiguration)
        {
            ServiceScopeFactory = serviceScopeFactory;
            S3FileProviderConfiguration = s3FileProviderConfiguration;
        }

        public IAmazonS3 Create()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var s3ClientFactory = scope.ServiceProvider.GetRequiredService<IS3ClientFactory>();
            return s3ClientFactory.Create(S3FileProviderConfiguration);
        }

        public bool Return(IAmazonS3 obj)
        {
            return obj != null;
        }
    }
}
