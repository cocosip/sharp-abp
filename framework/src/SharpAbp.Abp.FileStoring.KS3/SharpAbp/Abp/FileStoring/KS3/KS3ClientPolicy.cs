using KS3;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public class KS3ClientPolicy : IObjectPoolPolicy<IKS3>
    {
        protected IKS3ClientFactory KS3ClientFactory { get; }
        protected KS3FileProviderConfiguration KS3FileProviderConfiguration { get; }

        public KS3ClientPolicy(
            IKS3ClientFactory kS3ClientFactory,
            KS3FileProviderConfiguration kS3FileProviderConfiguration)
        {
            KS3ClientFactory = kS3ClientFactory;
            KS3FileProviderConfiguration = kS3FileProviderConfiguration;
        }

        public IKS3 Create()
        {
            return KS3ClientFactory.Create(KS3FileProviderConfiguration);
        }

        public bool Return(IKS3 obj)
        {
            return obj != null;
        }
    }
}
