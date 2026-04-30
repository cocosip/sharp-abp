using Aliyun.OSS;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public class AliyunOssClientPolicy : IObjectPoolPolicy<IOss>
    {
        protected IOssClientFactory OssClientFactory { get; }
        protected AliyunFileProviderConfiguration AliyunFileProviderConfiguration { get; }

        public AliyunOssClientPolicy(
            IOssClientFactory ossClientFactory,
            AliyunFileProviderConfiguration aliyunFileProviderConfiguration)
        {
            OssClientFactory = ossClientFactory;
            AliyunFileProviderConfiguration = aliyunFileProviderConfiguration;
        }

        public IOss Create()
        {
            return OssClientFactory.Create(AliyunFileProviderConfiguration);
        }

        public bool Return(IOss obj)
        {
            return obj != null;
        }
    }
}
