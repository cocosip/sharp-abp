using Aliyun.OSS;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public class AliyunOssClientPolicy : IObjectPoolPolicy<IOss>
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected AliyunFileProviderConfiguration AliyunFileProviderConfiguration { get; }

        public AliyunOssClientPolicy(
            IServiceScopeFactory serviceScopeFactory,
            AliyunFileProviderConfiguration aliyunFileProviderConfiguration)
        {
            ServiceScopeFactory = serviceScopeFactory;
            AliyunFileProviderConfiguration = aliyunFileProviderConfiguration;
        }

        public IOss Create()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var ossClientFactory = scope.ServiceProvider.GetRequiredService<IOssClientFactory>();
            return ossClientFactory.Create(AliyunFileProviderConfiguration);
        }

        public bool Return(IOss obj)
        {
            return obj != null;
        }
    }
}
