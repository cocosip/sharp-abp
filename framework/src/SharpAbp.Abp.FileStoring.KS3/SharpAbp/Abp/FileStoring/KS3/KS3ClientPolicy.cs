using KS3;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public class KS3ClientPolicy : IObjectPoolPolicy<IKS3>
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected KS3FileProviderConfiguration KS3FileProviderConfiguration { get; }

        public KS3ClientPolicy(
            IServiceScopeFactory serviceScopeFactory,
            KS3FileProviderConfiguration kS3FileProviderConfiguration)
        {
            ServiceScopeFactory = serviceScopeFactory;
            KS3FileProviderConfiguration = kS3FileProviderConfiguration;
        }

        public IKS3 Create()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var ks3ClientFactory = scope.ServiceProvider.GetRequiredService<IKS3ClientFactory>();
            return ks3ClientFactory.Create(KS3FileProviderConfiguration);
        }

        public bool Return(IKS3 obj)
        {
            return obj != null;
        }
    }
}
