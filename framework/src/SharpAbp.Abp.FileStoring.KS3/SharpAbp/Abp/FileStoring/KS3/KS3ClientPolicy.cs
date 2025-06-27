using System;
using KS3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public class KS3ClientPolicy : IPooledObjectPolicy<IKS3>
    {

        protected IServiceProvider ServiceProvider { get; }
        protected KS3FileProviderConfiguration KS3FileProviderConfiguration { get; }
        public KS3ClientPolicy(
            IServiceProvider serviceProvider,
            KS3FileProviderConfiguration kS3FileProviderConfiguration)
        {
            ServiceProvider = serviceProvider;
            KS3FileProviderConfiguration = kS3FileProviderConfiguration;
        }

        public IKS3 Create()
        {
            using var scope = ServiceProvider.CreateScope();
            var ks3ClientFactory = scope.ServiceProvider.GetRequiredService<IKS3ClientFactory>();
            return ks3ClientFactory.Create(KS3FileProviderConfiguration);
        }

        public bool Return(IKS3 obj)
        {
            return obj != null;
        }
    }
}
